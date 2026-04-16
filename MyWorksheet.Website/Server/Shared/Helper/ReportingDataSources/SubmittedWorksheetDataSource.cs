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

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class SubmittedWorksheetDataSource : IReportingDataSource
{
    class SubmittedWorksheetArguments : ArgumentsBase
    {
        public List<SubmittedWorksheetsReporting> Worksheets { get; set; }
    }

    public SubmittedWorksheetDataSource()
    {
        Id = new Guid("00000000-0000-0000-0004-000000000003");
        Key = "submittedWorksheetDataSource";
        Name = "Submitted Worksheets";
        Purpose = Array.Empty<ReportPurpose>();
    }
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }
    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<SubmittedWorksheetArguments>));
    }

    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        IDictionary<string, object> arguments)
    {
        var projectsOfUserQuery = db.Worksheets.Where(e => e.IdCurrentStatusNavigation.AllowModifications == false).Where(f => f.IdCreator == userId);
        projectsOfUserQuery = DataSourceHelper.TranslateQuery(projectsOfUserQuery, query);
        return new Dictionary<string, object>
        {
            {
                "DataSource", new Dictionary<string, object>()
                {
                    {
                        "Worksheets", projectsOfUserQuery.ToArray()
                    }
                }
            }
        };
    }

    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        var paraInfos = new List<ReportingParameterInfo>
        {
            new ReportingParameterInfo()
            {
                Type = typeof(int),
                AllowedOperators = new[] { ReportingOperators.EqualTo },
                AllowedValues = config.Projects.Where(f => f.IdCreator == userId).ToArray().Select(e => new ReportingParamterValue(e.Name, e.ProjectId)).ToArray(),
                Name = nameof(SubmittedWorksheetsReporting.IdProject),
                Display = "Project"
            }
        };
        DataSourceHelper.FillRemaining<SubmittedWorksheetsReporting>(paraInfos, userId,
            nameof(SubmittedWorksheetsReporting.IdCreator));
        return paraInfos.ToArray();
    }
}