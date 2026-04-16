using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class FullTreeDataSource : IReportingDataSource
{
    private readonly IMapperService _mapperService;

    class FullProjectReportArguments : ArgumentsBase
    {
        public List<FullProjectDataReporting> Projects { get; set; }
    }

    public FullTreeDataSource(IMapperService mapperService)
    {
        _mapperService = mapperService;

        Id = new Guid("00000000-0000-0000-0004-000000000004");
        Key = "FullDataTree";
        Name = "Full Project Data";
        Purpose = Array.Empty<ReportPurpose>();
    }

    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }
    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<FullProjectReportArguments>));
    }

    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        IDictionary<string, object> arguments)
    {
        var projectsOfUserQuery = db.Projects.Where(f => f.IdCreator == userId);
        var projects = projectsOfUserQuery.ToArray();

        return new Dictionary<string, object>
        {
            {
                nameof(ReportingDataStructureBase<object>.DataSource), new Dictionary<string, object>()
                {
                    {
                        nameof(FullProjectReportArguments.Projects), _mapperService.ViewModelMapper.Map<ProjectReporting[]>(projects).Select(e => new FullProjectDataReporting()
                        {
                            Project = e,
                            Worksheets = _mapperService.ViewModelMapper.Map<List<WorksheetReporting>>(db
                                    .Worksheets.Where(f => f.IdCreator == userId && e.ProjectId == f.IdProject).ToList())
                                .Select(f => new FullProjectWorksheetDataReporting()
                                {
                                    Worksheet = f,
                                    WorksheetItems = _mapperService.ViewModelMapper.Map<List<WorksheetItemReporting>>(
                                        db.WorksheetItems.Where(w => w.IdCreator == userId && w.IdWorksheet == f.WorksheetId)
                                        .ToList())
                                }).ToList()
                        })
                    }
                }
            }
        };
    }

    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        return DataSourceHelper.FillRemaining<Project>(new List<ReportingParameterInfo>(), userId, nameof(Project.IdCreator)).ToArray();
    }
}
