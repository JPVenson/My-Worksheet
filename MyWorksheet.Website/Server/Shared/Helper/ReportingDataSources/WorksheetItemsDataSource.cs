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

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class WorksheetItemsDataSource : IReportingDataSource
{
    class WorksheetItemsArguments : ArgumentsBase
    {
        public List<WorksheetItemReporting> WorksheetItems { get; set; }
    }

    public WorksheetItemsDataSource()
    {
        Name = "Worksheet Items";
        Key = "wsItemSource";
        Id = new Guid("00000000-0000-0000-0004-000000000002");
        Purpose = Array.Empty<ReportPurpose>();
    }

    public string Name { get; set; }
    public string Key { get; set; }
    public Guid Id { get; set; }
    public ReportPurpose[] Purpose { get; set; }

    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<WorksheetItemsArguments>)).ExtendComments(new Dictionary<string, string>
        {
            {"DataSource.WorksheetItems.FromTime", "The time this timespan has started in minutes"},
            {"DataSource.WorksheetItems.ToTime", "The time this timespan has ended in minutes"},
            {"DataSource.WorksheetItems.Hidden", "Marked as Hidden"},
            {"DataSource.WorksheetItems.Comment", "A freetext comment to this timespan"},
            {"DataSource.WorksheetItems.StatusCode", "The current Workflow Status"},
            {"DataSource.WorksheetItems.Timespan", "The difference of FromTime and ToTime"},
            {"DataSource.WorksheetItems.WorksheetActionsCsv", "CSV of all Status codes that are applyed"},
        });
    }

    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        var paraInfos = new List<ReportingParameterInfo>();
        var worksheets = config.Worksheets.Where(f => f.IdCreator == userId)
            .ToArray();

        var projects = config.Projects.Where(f => f.IdCreator == userId)
            .ToArray();

        paraInfos.Add(new ReportingParameterInfo
        {
            Name = nameof(WorksheetItemReporting.IdWorksheet),
            Display = "Worksheet",
            Type = typeof(int),
            AllowedOperators = new[] { ReportingOperators.EqualTo },
            AllowedValues = worksheets
                .OrderBy(f => f.StartTime.Year).ThenBy(f => f.StartTime.Month)
                .Select(f => new ReportingParamterValue(f.StartTime.Month + "/" + f.StartTime.Year, f.WorksheetId)).ToArray()
        });

        paraInfos.Add(new ReportingParameterInfo
        {
            Name = nameof(WorksheetItemReporting.ProjectId),
            Display = "Project",
            Type = typeof(int),
            AllowedOperators = new[] { ReportingOperators.EqualTo },
            AllowedValues = projects
                .Select(f => new ReportingParamterValue(f.Name, f.ProjectId)).ToArray()
        });
        DataSourceHelper.FillRemaining<WorksheetItemReporting>(paraInfos, userId,
            nameof(WorksheetItemReporting.IdCreator));
        return paraInfos.ToArray();
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId,
        ReportingExecutionParameterValue[] query, IDictionary<string, object> arguments)
    {
        var projectsOfUserQuery = db.Worksheets
            .Include(f => f.WorksheetAsserts)
            .Include(f => f.IdProjectNavigation)
            .Include(f => f.IdCurrentStatusNavigation)
            .Include(f => f.WorksheetItems)
            .Where(f => f.IdCreator == userId);

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
}