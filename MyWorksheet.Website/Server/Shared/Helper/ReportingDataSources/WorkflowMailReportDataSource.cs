using System.Collections.Generic;
using System;
using System.Linq;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class WorkflowMailReportDataSource : IReportingDataSource
{
    private readonly IMapperService _mapperService;

    public WorkflowMailReportDataSource(IMapperService mapperService)
    {
        _mapperService = mapperService;
        Purpose = new ReportPurpose[]
        {
            ReportPurposes.Worksheet
        };
    }
    /// <inheritdoc />
    public Guid Id { get; set; } = new Guid("00000000-0000-0000-0004-000000000004");

    /// <inheritdoc />
    public string Key { get; set; } = "WorkflowMailDataSource";

    /// <inheritdoc />
    public string Name { get; set; } = "Workflow Mail Data Source";
    public ReportPurpose[] Purpose { get; set; }

    /// <inheritdoc />
    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(new
        {
            DataSource = new
            {
                Worksheet = new WorksheetModel(),
                Project = new PostProjectModel(),
                Owner = new
                {
                    User = new AccountApiUserPost(),
                    Address = new AddressModel(),
                },
                Sender = new
                {
                    User = new AccountApiUserPost(),
                    Address = new AddressModel(),
                },

            }
        }).ExtendComments(new Dictionary<string, string>
        {
            {"DataSource.Worksheets.WorksheetId", "Primary Key"},
            {"DataSource.Worksheets.Hidden", "Marked as Hidden"},
            {"DataSource.Worksheets.StatusCode", "The current Workflow Status"},
        });
    }
    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        IDictionary<string, object> arguments)
    {
        var projectsOfUserQuery = db.Worksheets.Where(f => f.IdCreator == userId);
        projectsOfUserQuery = DataSourceHelper.TranslateQuery(projectsOfUserQuery, query);

        var worksheets = projectsOfUserQuery.ToArray();

        var worksheet = worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            return new Dictionary<string, object>();
        }

        var project = db.Projects.Find(worksheet.IdProject);
        var ownerOrg = db.Organisations.Find(project.IdOrganisation);
        Address ownerAddress = null;
        if (ownerOrg != null)
        {
            ownerAddress = db.Addresses.Find(ownerOrg.IdAddress);
        }
        var user = db.AppUsers.Find(userId);
        var address = db.Addresses.Find(user.IdAddress);


        return new Dictionary<string, object>
        {
            {
                "DataSource", new
                {
                    Worksheet = _mapperService.ViewModelMapper.Map<WorksheetModel>(worksheet),
                    Project = _mapperService.ViewModelMapper.Map<PostProjectModel>(project),
                    Owner = new
                    {
                        User = _mapperService.ViewModelMapper.Map<AccountApiUserPost>(ownerOrg),
                        Address =_mapperService.ViewModelMapper.Map<AddressModel>(ownerAddress),
                    },
                    Sender = new
                    {
                        User = _mapperService.ViewModelMapper.Map<AccountApiUserPost>(user),
                        Address =_mapperService.ViewModelMapper.Map<AddressModel>(address),
                    },
                }
            }
        };
    }

    /// <inheritdoc />
    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        var projs = config.Projects.Where(e => e.IdCreator == userId).ToArray();
        var worksheets = config.Worksheets.Where(e => e.IdCreator == userId).ToArray();
        return new[]
        {
            new ReportingParameterInfo()
            {
                Name = nameof(Models.Worksheet.WorksheetId),
                Type = typeof(int),
                AllowedOperators = new []{ ReportingOperators.EqualTo},
                AllowedValues = worksheets.Join(projs, e => e.IdProject, e => e.ProjectId,
                        (w,p) => new ReportingParamterValue(p.Name + " - " + w.StartTime.ToString("d") + "/" + w.EndTime.Value.ToString("d"), w.WorksheetId))
                    .ToArray(),
                Display = "Worksheet"
            },
        };
    }
}