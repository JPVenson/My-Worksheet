using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using Microsoft.EntityFrameworkCore;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class WorksheetDataSource : IReportingDataSource
{
    class WorksheetDataArguments : ArgumentsBase
    {
        public List<WorksheetReporting> Worksheets { get; set; }
    }

    public WorksheetDataSource(IMapperService mapperService)
    {
        Id = new Guid("00000000-0000-0000-0004-000000000003");
        Key = "worksheetDataSource";
        Name = "Worksheets";
        Purpose = new[]
        {
            ReportPurposes.Worksheet
        };
        MapperService = mapperService;
    }

    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }
    public IMapperService MapperService { get; }

    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<WorksheetDataArguments>))
            .ExtendComments(new Dictionary<string, string>
            {
                {"DataSource.Worksheets.WorksheetId", "Id"},
                {"DataSource.Worksheets.Hidden", "Marked as Hidden"},
                {"DataSource.Worksheets.StatusCodeKey", "The current Workflow Status"}
            });
    }

    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId,
        ReportingExecutionParameterValue[] query, IDictionary<string, object> arguments)
    {
        var projectsOfUserQuery = db.Worksheets
            .Include(f => f.WorksheetItemStatuses)
            .Where(f => f.IdCreator == userId);
        projectsOfUserQuery = DataSourceHelper.TranslateQuery(projectsOfUserQuery, query);
        return new Dictionary<string, object>
        {
            {
                "DataSource", new Dictionary<string, object>
                {
                    {
                        "Worksheets", MapperService.ViewModelMapper.Map<WorksheetReportingViewModel>(projectsOfUserQuery)
                    }
                }
            }
        };
    }

    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        var paraInfos = new List<ReportingParameterInfo>
        {
            new ReportingParameterInfo
            {
                Type = typeof(int),
                AllowedOperators = new[] { ReportingOperators.EqualTo },
                AllowedValues = config.Projects.Where(f => f.IdCreator == userId).ToArray()
                .Select(e => new ReportingParamterValue(e.Name, e.ProjectId)).ToArray(),
                Name = nameof(WorksheetReporting.IdProject),
                Display = "Project"
            }
        };
        DataSourceHelper.FillRemaining<WorksheetReporting>(paraInfos, userId,
            nameof(WorksheetReporting.IdCreator));
        return paraInfos.ToArray();
    }
}