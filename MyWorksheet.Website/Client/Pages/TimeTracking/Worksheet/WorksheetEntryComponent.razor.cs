using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public partial class WorksheetEntryComponent : NavigationPageBase
{
    [Parameter]
    public WorksheetEditViewModel Model { get; set; }

    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WhenChanged(Model.WorksheetItems).ThenRefresh(this);
        WhenChanged(Model.Worksheet).ThenRefresh(this);
        await Model.WorksheetItems.Load();
    }
}

public class WorksheetWeekDisplay : ViewModelBase
{
    private readonly WorksheetEditViewModel _model;
    public DateTimeOffset FirstDayOfWeek { get; set; }
    public int WeekNo { get; set; }

    public IList<WorksheetDayDisplay> WorksheetDays { get; set; }

    public WorksheetWeekDisplay(WorksheetEditViewModel model, DateTimeOffset firstDayOfWeek)
    {
        _model = model;
        FirstDayOfWeek = firstDayOfWeek;
        WeekNo = DateInfo.GetIso8601WeekOfYear(firstDayOfWeek);
        WorksheetDays = new List<WorksheetDayDisplay>();

        var day = firstDayOfWeek;
        for (int i = 0; i < 7; i++)
        {
            if (model.Worksheet.StartTime > day)
            {
                day = day.AddDays(1);
                continue;
            }
            if (model.Worksheet.EndTime.HasValue &&
                model.Worksheet.EndTime < day
                || WeekNo != DateInfo.GetIso8601WeekOfYear(day))
            {
                break;
            }

            var worksheetDayDisplay = new WorksheetDayDisplay(model, day.Date);

            worksheetDayDisplay.PropertyChanged += WorksheetDayDisplay_PropertyChanged;
            WorksheetDays.Add(worksheetDayDisplay);
            day = day.AddDays(1);
        }

        IsCollapsed = CurrentWeek;
    }

    private void WorksheetDayDisplay_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        SendPropertyChanged();
    }

    public bool IsCollapsed { get; set; }

    public bool CurrentWeek
    {
        get
        {
            return WorksheetDays.Any(f => f.Today);
        }
    }

    public int GetWeekWorktime()
    {
        return WorksheetDays.Select(e => e.GetDayWorktime()).Sum();
    }

    public IEnumerable<WorksheetItemModel> GetItemsInWeek()
    {
        return _model.WorksheetItems.Where(e => WeekNo == DateInfo.GetIso8601WeekOfYear(e.DateOfAction));
    }

    public int GetWeekMeanWorktime()
    {
        return WorksheetDays.Select(e => e.GetDayMeanWorktime()).Sum();
    }
}

public struct WorksheetItemSpan
{
    public bool StartsWithinRange { get; private set; }
    public bool IsIncludedInRange { get; private set; }
    public bool EndsWithinRange { get; private set; }

    public int StartRange { get; private set; }
    public int EndRange { get; private set; }

    public int StartValue { get; private set; }
    public int EndValue { get; private set; }

    public bool IsInRange
    {
        get
        {
            return StartsWithinRange || IsIncludedInRange || EndsWithinRange;
        }
    }

    public double GetOffsetOfRangePercent()
    {
        if (IsIncludedInRange)
        {
            return 0;
        }

        if (StartsWithinRange)
        {
            return ((StartValue - StartRange) / 60D * 100);
        }

        return 0;
    }

    public double GetLengthRangePercent()
    {
        if (IsIncludedInRange)
        {
            return 100;
        }

        if (StartsWithinRange)
        {
            return 100 - ((StartValue - StartRange) / 60D * 100);
        }

        return 100 - ((EndRange - EndValue) / 60D * 100);
    }

    public static WorksheetItemSpan CheckRange(WorksheetItemModel wsi, int startSpan, int endSpan)
    {
        return new WorksheetItemSpan()
        {
            StartsWithinRange = startSpan <= wsi.FromTime && endSpan >= wsi.FromTime,
            EndsWithinRange = startSpan <= wsi.ToTime && endSpan >= wsi.ToTime,
            IsIncludedInRange = startSpan >= wsi.FromTime && endSpan <= wsi.ToTime,
            StartRange = startSpan,
            EndRange = endSpan,
            StartValue = wsi.FromTime,
            EndValue = wsi.ToTime
        };
    }
}

public class WorksheetDayDisplay : ViewModelBase
{
    private readonly WorksheetEditViewModel _model;
    public DateTimeOffset Day { get; }

    public WorksheetDayDisplay(WorksheetEditViewModel model, DateTimeOffset day)
    {
        _model = model;
        Day = day;
        var times = GetWorksheetItems().Select(e => e.ToTime).ToArray();
        Editor = new WorksheetItemEditor();
        Editor.DateOfAction = Day;
        Editor.FromTime = times.Any() ? times.Max() : 0;

        if (Today && !times.Any())
        {
            var dateTimeOffset = DateTimeOffset.UtcNow;
            Editor.FromTime = dateTimeOffset.Hour * 60 + dateTimeOffset.Minute;
        }

        if (Editor.FromTime != 0)
        {
            Editor.ToTime = Editor.FromTime + 120;
        }

        IsWorkingDay = model.WorktimeMode.GetDayWorktime(model.Workload, day.DayOfWeek) > 0;
        Editor.PropertyChanged += EditorOnPropertyChanged;
    }

    private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        SendPropertyChanged();
    }

    public WorksheetItemEditor Editor { get; set; }

    public bool IsWorkingDay { get; set; }

    public bool Today
    {
        get
        {
            return Day == DateTimeOffset.UtcNow.Date;
        }
    }

    public bool IsPast
    {
        get
        {
            return Day < DateTimeOffset.UtcNow.Date;
        }
    }

    public bool IsFuture
    {
        get
        {
            return Day > DateTimeOffset.UtcNow.Date;
        }
    }

    public int GetDayMeanWorktime()
    {
        return (int)_model.WorktimeMode.GetDayWorktime(_model.Workload, Day.DayOfWeek);
    }

    public int GetDayWorktime()
    {
        return GetWorksheetItems().Select(f => f.ToTime - f.FromTime).Sum();
    }

    public IEnumerable<WorksheetItemModel> GetWorksheetItems()
    {
        return _model.WorksheetItems.Where(e => e.DateOfAction.Date == Day).OrderBy(e => e.FromTime);
    }

    public IEnumerable<WorksheetItemModel> GetWorksheetItemsWithPause(bool leadingPause = false, bool tailingPause = false)
    {
        WorksheetItemModel lastWsModel = null;
        var worksheetItemModels = GetWorksheetItems().ToArray();
        var first = worksheetItemModels.FirstOrDefault();
        var last = worksheetItemModels.LastOrDefault();
        if (leadingPause && first != null && first.FromTime != 0)
        {
            yield return CreatePauseItem(0, first.ToTime);
        }
        foreach (var worksheetItemModel in worksheetItemModels)
        {
            if (lastWsModel != null)
            {
                if (lastWsModel.ToTime < worksheetItemModel.FromTime)
                {
                    yield return CreatePauseItem(lastWsModel.ToTime, worksheetItemModel.FromTime);
                }
            }

            lastWsModel = worksheetItemModel;
            yield return worksheetItemModel;
        }
        if (tailingPause && last != null && last.FromTime != (24 * 60))
        {
            yield return CreatePauseItem(last.ToTime, 24 * 60);
        }
    }

    private static PauseWorksheetItemModel CreatePauseItem(int fromTime, int toTime)
    {
        return new PauseWorksheetItemModel()
        {
            FromTime = fromTime,
            ToTime = toTime,
            Comment = "Pause",
        };
    }
}

public class PauseWorksheetItemModel : WorksheetItemModel
{

}

public class WorksheetItemEditor : ViewModelBase
{
    private int _fromTime;
    private int _toTime;
    private string _comment;
    public DateTimeOffset DateOfAction { get; set; }

    public string Comment
    {
        get { return _comment; }
        set { SetProperty(ref _comment, value); }
    }

    [Required]
    [Range(1, 1439)]
    public int ToTime
    {
        get { return _toTime; }
        set { SetProperty(ref _toTime, value); }
    }

    [Required]
    [Range(2, 1440)]
    public int FromTime
    {
        get { return _fromTime; }
        set { SetProperty(ref _fromTime, value); }
    }

    public Guid? IdProjectItemRate { get; set; }

    public override int State()
    {
        return HashCode.Combine(IdProjectItemRate, FromTime, ToTime, Comment);
    }
}

public class WorksheetItemEditorBatchEdit : WorksheetItemEditor
{
    private bool _overlaps;

    public bool Overlaps
    {
        get { return _overlaps; }
        set { SetProperty(ref _overlaps, value); }
    }
}