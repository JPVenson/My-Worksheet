using System;
using System.Collections.Generic;
using System.Linq;
using Katana.CommonTasks.Extentions;
using MyWorksheet.Helper.Db;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Website.Server.Services.Worktime;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Statistics.Provider;

public class OvertimeStatisticsProvider : StatisticsProviderBase
{
    private readonly WorksheetWorkflowManager _worksheetWorkflowManager;
    private IUserWorktimeService _userWorktimeService;
    private ILogger<OvertimeStatisticsProvider> _appLogger;

    public OvertimeStatisticsProvider(IDbContextFactory<MyworksheetContext> dbContextFactory,
        WorksheetWorkflowManager worksheetWorkflowManager,
        IUserWorktimeService userWorktimeService,
        ILogger<OvertimeStatisticsProvider> appLogger) : base(dbContextFactory)
    {
        _worksheetWorkflowManager = worksheetWorkflowManager;
        _userWorktimeService = userWorktimeService;
        _appLogger = appLogger;
    }

    public override string Display { get; } = "Overtime";

    public static IQueryable<Worksheet> WorksheetsInRange(Guid projectId, DateTimeOffset? fromDate,
        DateTimeOffset? toDate,
        bool includeVolantileData,
        MyworksheetContext db)
    {
        return WorksheetsInRange(new[] { projectId }, fromDate, toDate, includeVolantileData, db);
    }

    public static IQueryable<Worksheet> WorksheetsInRange(Guid[] projectId,
        DateTimeOffset? fromDate,
        DateTimeOffset? toDate,
        bool includeVolantileData,
        MyworksheetContext db)
    {
        if (!fromDate.HasValue)
        {
            fromDate = DateTimeOffset.UtcNow.AddMonths(-12 * 3);
        }
        if (!toDate.HasValue)
        {
            toDate = fromDate.Value.AddMonths(12 * 3);
        }

        var worksheetsQuery = db.Worksheets.Where(f => projectId.Contains(f.IdProject));

        if (!includeVolantileData)
        {
            worksheetsQuery = worksheetsQuery.Where(e => e.IdCurrentStatus > WorksheetStatusType.Created.ConvertToGuid());
        }

        worksheetsQuery = worksheetsQuery.Where(e => e.StartTime >= fromDate.Value && e.EndTime < toDate.Value);

        return worksheetsQuery;
    }

    public override IObjectSchema Arguments(MyworksheetContext db)
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ActionCasesStatisticsProvider.ActionCasesStatisticsArguments));
    }
    public const string FormatDtPoint = "MMM/yy";

    public override DataExport DataExport(Guid appUserId, IDictionary<string, object> arguments, Guid projects)
    {
        var db = DbContextFactory.CreateDbContext();

        var fromDate = (DateTimeOffset?)arguments.GetOrNull("FromDate");
        var toDate = (DateTimeOffset?)arguments.GetOrNull("ToDate");
        bool includeVolantileData = ((bool?)arguments.GetOrNull("IncludeNonSubmitted")) ?? false;

        var projectData = db.Projects.Where(e => e.ProjectId == projects).First();

        if (projectData.IdCreator != appUserId)
        {
            _appLogger.LogCritical("Prevented attempted id injection occured! Method: GetProjectData.1", "Security",
                new Dictionary<string, string>
                {
                    {
                        "projectId", projectData.ProjectId.ToString()
                    },
                    {
                        "UserId", appUserId.ToString()
                    }
                });

            return null;
        }

        var worksheetsQuery = WorksheetsInRange(new Guid[] { projects }, fromDate, toDate, includeVolantileData, DbContextFactory.CreateDbContext());

        var worksheets = worksheetsQuery.ToArray();

        var de = new DataExport();

        if (worksheets.Length == 0)
        {
            return de;
        }

        WorksheetItem[] worksheetDataItems;
        Dictionary<int, Dictionary<int, DateTime>> yearLabels;
        PopulateWorksheetItems(db, worksheets, de, out worksheetDataItems, out yearLabels);

        var hoursWorkedSerie = new DataDimension(new LabelOversight("Hours Worked"));
        hoursWorkedSerie.DataSamples.Add(new DataSample(new LabelOversight("Hours")));
        hoursWorkedSerie.DataSamples.Add(new DataSample(new LabelOversight("Aggregated")));
        de.DataDimensions.Add(hoursWorkedSerie);

        //var earnedLostSerie = new DataDimension(new LabelOversight("Earned/Lost"));
        //earnedLostSerie.DataSamples.Add(new DataSample(new LabelOversight("€")));
        //earnedLostSerie.DataSamples.Add(new DataSample(new LabelOversight("+/-")));
        //de.DataDimensions.Add(earnedLostSerie);

        decimal hoursAggregated = 0;

        foreach (var yearLabel in yearLabels)
        {
            foreach (var months in yearLabel.Value)
            {
                Dictionary<int, decimal> meanWorktimeMap = PopulateWeekWorktime(db, projectData, appUserId, _userWorktimeService);
                decimal shouldHoursWorked = 0;
                var daysInMonth = DateTime.DaysInMonth(yearLabel.Key, months.Key);
                for (int i = 1; i < daysInMonth; i++)
                {
                    var doM = new DateTime(yearLabel.Key, months.Key, i);
                    var mwt = meanWorktimeMap[(int)doM.DayOfWeek];
                    shouldHoursWorked += mwt;
                }

                var worksheet = worksheets.Where(f => f.StartTime.Year == yearLabel.Key && f.StartTime.Month == months.Key).ToArray();
                if (!worksheet.Any())
                {
                    hoursWorkedSerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s"))));
                    hoursWorkedSerie.DataSamples[1].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), Math.Round(hoursAggregated, 2)));

                    //earnedLostSerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), 0D));
                    //earnedLostSerie.DataSamples[1].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), hoursAggregated));
                    continue;
                }

                var worksheetItems = worksheetDataItems.Where(f => worksheet.Any(e => e.WorksheetId == f.IdWorksheet)).ToArray();
                var hoursWorked = worksheetItems.Select(f => f.ToTime - f.FromTime).AggregateIf(e => e.Any(), (e, f) => e + f) / 60;
                var overtime = hoursWorked - shouldHoursWorked / 60;
                hoursAggregated += overtime;
                hoursWorkedSerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), Math.Round(overtime, 2)));
                hoursWorkedSerie.DataSamples[1].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), Math.Round(hoursAggregated, 2)));

                //earnedLostSerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), overtime * (decimal)projectData.Honorar));
                //earnedLostSerie.DataSamples[1].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), hoursAggregated * (decimal)projectData.Honorar));
            }
        }
        return de;
    }

    public static Dictionary<int, decimal> PopulateWeekWorktime(MyworksheetContext db, Project projectData, Guid userId,
        IUserWorktimeService userWorktimeService)
    {
        var workloadForProject = userWorktimeService.GetWorkloadForProject(db, projectData.ProjectId, userId);

        var meanWorktimeMap = new Dictionary<int, decimal>
        {
            { 0, workloadForProject.MeanWorktimeForDay(DayOfWeek.Saturday) },
            { 1, workloadForProject.MeanWorktimeForDay(DayOfWeek.Monday) },
            { 2, workloadForProject.MeanWorktimeForDay(DayOfWeek.Tuesday) },
            { 3, workloadForProject.MeanWorktimeForDay(DayOfWeek.Wednesday) },
            { 4, workloadForProject.MeanWorktimeForDay(DayOfWeek.Thursday) },
            { 5, workloadForProject.MeanWorktimeForDay(DayOfWeek.Friday) },
            { 6, workloadForProject.MeanWorktimeForDay(DayOfWeek.Saturday) }
        };
        return meanWorktimeMap;
    }

    public static void PopulateWorksheetItems(MyworksheetContext db, Worksheet[] worksheets, DataExport de,
        out WorksheetItem[] worksheetDataItems,
        out Dictionary<int, Dictionary<int, DateTime>> yearLabels)
    {
        worksheetDataItems = db.WorksheetItems.Where(f => worksheets.Select(f => f.WorksheetId).Contains(f.IdWorksheet)).ToArray();
        var minYear = worksheets.Select(e => e.StartTime).Min();
        var maxYear = worksheets.Select(e => e.EndTime).Max() ?? minYear;


        var maxDate = maxYear;//.AddMonths(1);
        var minDate = minYear;//.AddMonths(1);
        var maxYears = 3;

        yearLabels = [];
        do
        {
            var mon = new Dictionary<int, DateTime>();
            for (var i = minDate.Month; i < 13; i++)
            {
                mon.Add(i, new DateTime(minDate.Year, i, 1));
                if (i == maxDate.Month && minDate.Year == maxDate.Year)
                {
                    break;
                }
            }

            yearLabels.Add(minDate.Year, mon);
            minDate = minDate.AddMonths(13 - minDate.Month);
            maxYears--;
        } while (minDate.Year <= maxDate.Year && maxYears > 0);

        if (maxYears == 0)
        {
            de.DataMaybeTruncated = true;
        }
    }
}