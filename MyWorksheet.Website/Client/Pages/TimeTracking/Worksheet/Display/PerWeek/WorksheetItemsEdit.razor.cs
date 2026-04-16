using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Collapse;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Display.PerWeek;

public partial class WorksheetItemsEdit
{
    public WorksheetItemsEdit()
    {
    }

    [Parameter]
    public WorksheetEditViewModel Model { get; set; }
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public ChargeRateService ChargeRateService { get; set; }
    [Inject]
    public WorksheetTrackerService WorksheetTrackerService { get; set; }

    public IEnumerable<ProjectItemRateViewModel> ItemRates { get; set; }

    public WorksheetTimeTrackerViewModel Tracker { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        ItemRates = await ChargeRateService.GetRatesForProject(Model.Project.ProjectId);

        // WhenChanged(Model.WorksheetItems)
        // 	.ThenRefresh(this);

        WhenChanged(WorksheetTrackerService)
            .Then(() =>
            {
                Tracker = WorksheetTrackerService.Trackers.FirstOrDefault(e => e.IdWorksheet == Model.Worksheet.WorksheetId);
                Render();
            })
            .Trigger();
    }
}