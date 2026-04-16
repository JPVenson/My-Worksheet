using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Helper;
using MyWorksheet.Helper.Db;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Worktime;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Statistics.Provider;

public class ActionCasesStatisticsProvider : StatisticsProviderBase
{
    private readonly IAppLogger _appLogger;
    private readonly IUserWorktimeService _userWorktimeService;

    public ActionCasesStatisticsProvider(IDbContextFactory<MyworksheetContext> dbContextFactory, IAppLogger appLogger,
        IUserWorktimeService userWorktimeService) : base(dbContextFactory)
    {
        _appLogger = appLogger;
        _userWorktimeService = userWorktimeService;
    }

    public class ActionCasesStatisticsArguments : ArgumentsBase
    {
        [JsonComment("Statistics/ActionCases.Arguments.Comments.FromDate")]
        [JsonDisplayKey("Statistics/ActionCases.Arguments.Names.FromDate")]
        public DateTimeOffset? FromDate { get; set; }

        [JsonComment("Statistics/ActionCases.Arguments.Comments.ToDate")]
        [JsonDisplayKey("Statistics/ActionCases.Arguments.Names.ToDate")]
        public DateTimeOffset? ToDate { get; set; }

        [JsonComment("Statistics/ActionCases.Arguments.Comments.IncludenNonSub")]
        [JsonDisplayKey("Statistics/ActionCases.Arguments.Names.IncludeNonSub")]
        public bool? IncludeNonSubmitted { get; set; }
    }

    public override string Display { get; } = "Worksheet Actions";
    public override IObjectSchema Arguments(MyworksheetContext db)
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ActionCasesStatisticsArguments));
    }

    public override DataExport DataExport(Guid appUserId, IDictionary<string, object> arguments, Guid projects)
    {
        var db = DbContextFactory.CreateDbContext();
        DateTime? fromDate = (DateTime?)arguments.GetOrNull("FromDate");
        DateTime? toDate = (DateTime?)arguments.GetOrNull("ToDate");
        bool includeVolantileData = ((bool?)arguments.GetOrNull("IncludeNonSubmitted")) ?? false;

        var projectData = db.Projects.Find(projects);

        if (projectData.IdCreator != appUserId)
        {
            _appLogger.LogCritical("Prevented attempted id injection occured! Method: GetProjectData.1", "Security",
                new Dictionary<string, string>
                {
                    {
                        "projectId", projects.ToString()
                    },
                    {
                        "UserId", appUserId.ToString()
                    }
                });

            return null;
        }
        var de = new DataExport();

        DataSample occurences;
        DataDimension occurencesByDays = null;
        //DataSample moneyActions;
        DataSample timeActions;
        de.DataDimensions.Add(new DataDimension(new LabelOversight("Item Actions"))
        {
            DisplayAs = DisplayTypes.Bar,
            DataSamples =
            {
                (occurences = new DataSample(new LabelOversight("Occurrences")))
            }
        });
        //de.DataDimensions.Add(occurencesByDays = new DataDimension(new LabelOversight("Item Actions by Weekdays"))
        //{
        //	DisplayAs = DisplayTypes.Bar,
        //});
        de.DataDimensions.Add(new DataDimension(new LabelOversight("Item Time Lost"))
        {
            DisplayAs = DisplayTypes.Bar,
            DataSamples =
            {
                (timeActions = new DataSample(new LabelOversight("Hours"))),
            }
        });
        //de.DataDimensions.Add(new DataDimension(new LabelOversight("Item Money Lost"))
        //{
        //	DisplayAs = DisplayTypes.Bar,
        //	DataSamples =
        //		{
        //				(moneyActions = new DataSample(new LabelOversight("€"))),
        //		}
        //});

        var worksheetsQuery = OvertimeStatisticsProvider.WorksheetsInRange(projects, fromDate, toDate, includeVolantileData, db);
        var worksheets = worksheetsQuery.Select(e => e.WorksheetId).ToArray();

        var statusKeys = db.WorksheetItemStatuses.Where(f => worksheets.Contains(f.IdWorksheet)).ToArray();
        var statusLookup = db.WorksheetItemStatusLookups.Where(f => f.IdAppUser == appUserId || f.IdAppUser == null).ToArray();

        var workloadForProject = _userWorktimeService.GetWorkloadForProject(db, projects, appUserId);

        foreach (var statusGroup in statusKeys.GroupBy(e => e.IdWorksheetItemStatusLookup))
        {
            var lookup = statusLookup.First(e => e.WorksheetItemStatusLookupId == statusGroup.Key);
            occurences.DataLines.Add(new DataLine(new LabelOversight(lookup.Description), new LabelOversight(lookup.Description), statusGroup.Count()));
            var timeMissed = statusGroup.Select(e => workloadForProject.MeanWorktimeForDay(e.DateOfAction.DayOfWeek))
                .Aggregate((e, f) => e + f);
            timeActions.DataLines.Add(new DataLine(new LabelOversight(lookup.Description), new LabelOversight(lookup.Description), timeMissed / 60));
            //moneyActions.DataLines.Add(new DataLine(new LabelOversight(lookup.Description), new LabelOversight(lookup.Description), (timeMissed * (decimal)projectData.Honorar) / 60));
        }

        foreach (var statusGroup in statusKeys.GroupBy(e => e.IdWorksheetItemStatusLookup))
        {
            var lookup = statusLookup.First(e => e.WorksheetItemStatusLookupId == statusGroup.Key);
            var item = new DataSample(new LabelOversight(lookup.Description));
            foreach (var day in statusGroup.GroupBy(e => e.DateOfAction.DayOfWeek).OrderBy(e => e.Key))
            {
                item.DataLines.Add(
                    new DataLine(new LabelOversight(day.Key.ToString()),
                        new LabelOversight(((int)day.Key).ToString()), day.Count()));
            }
            occurencesByDays?.DataSamples.Add(item);
        }




        //foreach (var datesWithAction in datesWithActions)
        //{
        //	foreach (var withAction in datesWithAction.Value)
        //	{
        //		var item = new DataSample(new LabelOversight(withAction.Key));
        //		item.DataLines.Add(
        //			new DataLine(new LabelOversight(datesWithAction.Key.ToString()),
        //			new LabelOversight(((int)datesWithAction.Key).ToString()), withAction.Value.Count));
        //		occurencesByDays.DataSamples.Add(item);
        //	}
        //}

        //DataSample day;
        //occurencesByDays.DataSamples.Add(day = new DataSample(new LabelOversight(sortedByDay.Key)));
        //foreach (var statusGroup in sortedByDay.Value.GroupBy(e => e.DayOfWeek).OrderBy(e => e.Key))
        //{
        //	day.DataLines.Add(new DataLine(new LabelOversight(statusGroup.Key.ToString()), new LabelOversight(statusGroup.Key.ToString()), statusGroup.Count()));
        //}

        return de;
    }
}

//var worksheetItemsStatusReportings = db.WorksheetItemsStatusReportings.Where.Column(f => f.ProjectId).Is
//                                       .EqualsTo(projectData.ProjectId).ToArray();

//var dataLine = new DataDimension(new LabelOversight("Item Actions"));
//de.DataDimensions.Add(dataLine);
//var sample = new DataSample(new LabelOversight("Occurrences"));
//dataLine.DataSamples.Add(sample);

//foreach (var worksheetItemsStatusReporting in worksheetItemsStatusReportings)
//{
//	sample.DataLines.Add(new DataLine(new LabelOversight(worksheetItemsStatusReporting.Description), worksheetItemsStatusReporting.Counts.GetValueOrDefault()));
//}