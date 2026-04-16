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

public class WorksheetItemsPerDayDataSource : IReportingDataSource
{
    class WorksheetItemsPerDayArguments : ArgumentsBase
    {
        public List<PerDayReporting> WorksheetItems { get; set; }
    }

    public WorksheetItemsPerDayDataSource()
    {
        Id = new Guid("00000000-0000-0000-0004-000000000004");
        Key = "perDayWorksheetItemsDataSource";
        Name = "Worksheet Items Per Day Aggregated";
        Purpose = Array.Empty<ReportPurpose>();
    }

    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }

    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<WorksheetItemsPerDayArguments>));
    }

    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        IDictionary<string, object> arguments)
    {
        var projectsOfUserQuery = db.WorksheetItems
            .Where(f => f.IdCreator == userId)
            .GroupBy(e => new { e.IdWorksheet, e.DateOfAction, e.IdWorksheetNavigation.IdCurrentStatus, e.IdWorksheetNavigation.IdProject })
            .Select(f => new PerDayReporting()
            {
                IdWorksheet = f.Key.IdWorksheet,
                DateOfAction = f.Key.DateOfAction,
                FromTime = f.Min(e => e.FromTime),
                ToTime = f.Max(e => e.ToTime),
                ProjectId = f.Key.IdProject,
                IdCurrentStatus = f.Key.IdCurrentStatus,
                IdCreator = userId,
                WorksheetActionsCsv = null // populated when querying the PerDayReporting view directly
            });
        projectsOfUserQuery = DataSourceHelper.TranslateQuery(projectsOfUserQuery, query);
        return new Dictionary<string, object>
        {
            {
                "DataSource", new Dictionary<string, object>
                {
                    {
                        "WorksheetItems", projectsOfUserQuery.ToArray()
                    }
                }
            }
        };
    }

    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        var paraInfos = new List<ReportingParameterInfo>();
        DataSourceHelper.FillRemaining<PerDayReporting>(paraInfos, userId,
            nameof(PerDayReporting.IdCreator));
        return paraInfos.ToArray();
    }

}