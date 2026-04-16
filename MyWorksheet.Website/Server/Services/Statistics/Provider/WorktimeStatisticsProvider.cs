using System;
using System.Collections.Generic;
using System.Linq;
using Katana.CommonTasks.Extentions;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Worktime;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Statistics.Provider;

public class WorktimeStatisticsProvider : StatisticsProviderBase
{
    private readonly IAppLogger _appLogger;
    private readonly IUserWorktimeService _userWorktimeService;

    public WorktimeStatisticsProvider(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IAppLogger appLogger,
        IUserWorktimeService userWorktimeService) : base(dbContextFactory)
    {
        _appLogger = appLogger;
        _userWorktimeService = userWorktimeService;
    }

    public override string Display { get; } = "Worktime";

    public override IObjectSchema Arguments(MyworksheetContext db)
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ActionCasesStatisticsProvider.ActionCasesStatisticsArguments));
    }

    public override DataExport DataExport(Guid appUserId, IDictionary<string, object> arguments, Guid projectId)
    {
        var db = DbContextFactory.CreateDbContext();

        var fromDate = (DateTimeOffset?)arguments.GetOrNull("FromDate");
        var toDate = (DateTimeOffset?)arguments.GetOrNull("ToDate");
        bool includeVolatileData = ((bool?)arguments.GetOrNull("IncludeNonSubmitted")) ?? false;

        var projectData = db.Projects.Where(e => e.ProjectId == projectId).First();

        if (projectData.IdCreator != appUserId)
        {
            _appLogger.LogCritical("Prevented attempted id injection occured! Method: GetProjectData.1", "Security",
                new Dictionary<string, string>
                {
                    { "projectId", projectId.ToString() },
                    { "UserId", appUserId.ToString() }
                });
            return null;
        }

        var worksheetsQuery = OvertimeStatisticsProvider.WorksheetsInRange(projectId, fromDate, toDate, includeVolatileData, db);
        var worksheets = worksheetsQuery.ToArray();
        var de = new DataExport();

        var hoursWorkedSerie = new DataDimension(new LabelOversight("Hours Worked"));
        hoursWorkedSerie.DataSamples.Add(new DataSample(new LabelOversight("Hours")));
        hoursWorkedSerie.DataSamples.Add(new DataSample(new LabelOversight("Mean Worktime")));

        var byDaySerie = new DataDimension(new LabelOversight("By day"));
        byDaySerie.DataSamples.Add(new DataSample(new LabelOversight("Hours")));
        byDaySerie.DataSamples.Add(new DataSample(new LabelOversight("Mean Worktime")));

        de.DataDimensions.Add(hoursWorkedSerie);
        de.DataDimensions.Add(byDaySerie);

        if (worksheets.Length == 0)
        {
            return de;
        }

        WorksheetItem[] worksheetDataItems;
        Dictionary<int, Dictionary<int, DateTime>> yearLabels;
        OvertimeStatisticsProvider.PopulateWorksheetItems(db, worksheets, de, out worksheetDataItems, out yearLabels);

        foreach (var yearLabel in yearLabels)
        {
            foreach (var months in yearLabel.Value)
            {
                var meanWorktimeMap = OvertimeStatisticsProvider.PopulateWeekWorktime(db, projectData, appUserId, _userWorktimeService);
                decimal shouldHoursWorked = 0;
                var byDaySerieDataPoints = new Dictionary<DateTime, DataLine>();

                for (int i = 1; i <= DateTime.DaysInMonth(yearLabel.Key, months.Key); i++)
                {
                    var doM = new DateTime(yearLabel.Key, months.Key, i);
                    var mwt = meanWorktimeMap[(int)doM.DayOfWeek];
                    shouldHoursWorked += mwt;
                    byDaySerieDataPoints[doM] = mwt > 0
                        ? new DataLine(new LabelOversight(doM.ToString("M/d/yy")), new LabelOversight(doM.ToString("s")), mwt / 60)
                        : new DataLine(new LabelOversight(doM.ToString("M/d/yy")), new LabelOversight(doM.ToString("s")), 0);
                }

                var worksheet = worksheets.Where(f =>
                    f.StartTime.Year <= yearLabel.Key && (f.EndTime?.Year ?? yearLabel.Key) >= yearLabel.Key &&
                    f.StartTime.Month <= months.Key && (f.EndTime?.Month ?? months.Key) >= months.Key)
                    .ToArray();

                if (!worksheet.Any())
                {
                    hoursWorkedSerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(OvertimeStatisticsProvider.FormatDtPoint)), new LabelOversight(months.Value.ToString("s"))));
                    hoursWorkedSerie.DataSamples[1].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(OvertimeStatisticsProvider.FormatDtPoint)), new LabelOversight(months.Value.ToString("s"))));

                    foreach (var dataPoint in byDaySerieDataPoints)
                    {
                        byDaySerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(dataPoint.Key.ToString("M/d/yy")), new LabelOversight(months.Value.ToString("s"))));
                        byDaySerie.DataSamples[1].DataLines.Add(dataPoint.Value);
                    }
                    continue;
                }

                var worksheetItems = worksheetDataItems.Where(f => worksheet.Any(e => e.WorksheetId == f.IdWorksheet)).ToArray();
                var hoursWorked = worksheetItems.Select(f => (decimal)(f.ToTime - f.FromTime))
                    .AggregateIf(e => e.Any(), (e, f) => e + f) / 60;

                hoursWorkedSerie.DataSamples[0].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(OvertimeStatisticsProvider.FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), hoursWorked));
                hoursWorkedSerie.DataSamples[1].DataLines.Add(new DataLine(new LabelOversight(months.Value.ToString(OvertimeStatisticsProvider.FormatDtPoint)), new LabelOversight(months.Value.ToString("s")), shouldHoursWorked / 60));

                foreach (var byDaySerieDataPoint in byDaySerieDataPoints)
                {
                    var workingTime = worksheetItems
                        .Where(e => e.DateOfAction.Date == byDaySerieDataPoint.Key.Date)
                        .ToArray();

                    double wt = double.NaN;
                    if (workingTime.Any())
                    {
                        wt = workingTime.Select(f => (double)(f.ToTime - f.FromTime)).Aggregate((e, f) => e + f) / 60D;
                    }
                    byDaySerie.DataSamples[0].DataLines.Add(
                        new DataLine(
                            new LabelOversight(byDaySerieDataPoint.Key.ToString("M/d/yy")),
                            new LabelOversight(byDaySerieDataPoint.Key.ToString("s")),
                            wt));
                    byDaySerie.DataSamples[1].DataLines.Add(byDaySerieDataPoint.Value);
                }
            }
        }

        return de;
    }
}