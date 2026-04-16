using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Dialog;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Shared;

public partial class WorksheetTrackEditorDialog
{
}

public class WorksheetTrackEditorDialogViewModel : DialogViewModelBase
{
    private readonly WorksheetTimeTrackerViewModel _tracker;
    private readonly ICacheRepository<GetProjectModel> _projectsCacheRepository;
    private readonly ICacheRepository<WorksheetModel> _worksheetCacheRepository;

    public WorksheetTrackEditorDialogViewModel(WorksheetTimeTrackerViewModel tracker,
        ICacheRepository<GetProjectModel> projectsCacheRepository,
        ICacheRepository<WorksheetModel> worksheetCacheRepository,
        WorksheetTrackerService worksheetTrackerService,
        WaiterService waiterService)
    {
        WorksheetTrackerService = worksheetTrackerService;
        WaiterService = waiterService;
        _tracker = tracker;
        _projectsCacheRepository = projectsCacheRepository;
        _worksheetCacheRepository = worksheetCacheRepository;
        ServerErrorManager = new ServerErrorManager(waiterService);
    }

    public GetProjectModel Project { get; set; }
    public WorksheetModel Worksheet { get; set; }
    public WorksheetTimeTrackerViewModel Tracker { get; set; }
    public WorksheetTrackerService WorksheetTrackerService { get; set; }
    public WaiterService WaiterService { get; set; }
    public ServerErrorManager ServerErrorManager { get; set; }

    public async Task Save()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var result = await WorksheetTrackerService.EndTracker(Tracker.IdWorksheet,
                Tracker.StartTime,
                Tracker.EndTime,
                Tracker.Comment);
            if (result.Success)
            {
                await Close();
                return;
            }

            ServerErrorManager.Eval(result);
        }
    }
}