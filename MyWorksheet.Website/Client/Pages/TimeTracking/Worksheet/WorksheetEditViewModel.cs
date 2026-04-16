using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public class WorksheetEditViewModel : ViewModelBase
{
    public WorksheetEditViewModel()
    {
        Weeks = new List<WorksheetWeekDisplay>();
    }

    public GetProjectModel Project { get; set; }
    public WorksheetModel Worksheet { get; set; }
    public IFutureTrackedList<WorksheetItemModel> WorksheetItems { get; set; }
    public ICollection<ProjectItemRateViewModel> ChargeRates { get; set; }
    public GetUserWorkloadViewModel Workload { get; set; }
    public IWorktimeMode WorktimeMode { get; set; }

    public IList<WorksheetWeekDisplay> Weeks { get; set; }
    public WorksheetWorkflowModel Workflow { get; set; }
    public WorksheetWorkflowDataMapViewModel WorkflowDataSet { get; set; }
    public IFutureList<WorksheetStatusModel> WorkflowHistory { get; set; }
    public IFutureList<WorksheetWorkflowStepViewModel> WorkflowActions { get; set; }

    public async Task RebuildWorksheetItems()
    {
        var weeks = new List<WorksheetWeekDisplay>();

        if (Worksheet.EndTime.HasValue)
        {
            var firstDayInRange = Worksheet.StartTime;
            while (firstDayInRange <= Worksheet.EndTime.Value)
            {
                System.Console.WriteLine($"Add week from: {firstDayInRange} no {DateInfo.GetIso8601WeekOfYear(firstDayInRange)}");
                var worksheetWeekDisplay = new WorksheetWeekDisplay(this, firstDayInRange);
                if (Weeks.All(f => f.WeekNo != worksheetWeekDisplay.WeekNo))
                {
                    weeks.Add(worksheetWeekDisplay);
                }
                while (worksheetWeekDisplay.WeekNo == DateInfo.GetIso8601WeekOfYear(firstDayInRange))
                {
                    System.Console.WriteLine($"Add D+1 to {firstDayInRange} as its in week {worksheetWeekDisplay.WeekNo}");
                    firstDayInRange = firstDayInRange.AddDays(1);
                }
            }
            System.Console.WriteLine($"Done. Last week is {DateInfo.GetIso8601WeekOfYear(firstDayInRange)} because {firstDayInRange} <= {Worksheet.EndTime.Value}");
        }
        else
        {
            await WorksheetItems.Load();
            foreach (var itemsInWeek in WorksheetItems
                    .GroupBy(e => DateInfo.GetIso8601WeekOfYear(e.DateOfAction.Date)))
            {
                if (Weeks.Any(f => f.WeekNo == itemsInWeek.Key))
                {
                    continue;
                }
                var day = itemsInWeek.OrderBy(e => e.DateOfAction.Day).First().DateOfAction.Date;
                while (day.DayOfWeek != DateInfo.TargetCulture.DateTimeFormat.FirstDayOfWeek)
                {
                    day = day.AddDays(-1);
                }
                var weekDisplay = new WorksheetWeekDisplay(this, day.Date);
                weeks.Add(weekDisplay);
            }
        }

        foreach (var worksheetWeekDisplay in weeks)
        {
            Weeks.Add(worksheetWeekDisplay);
        }

        Weeks = Weeks.OrderBy(e => e.WeekNo).ToList();
    }
}