using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Logging;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Server;

public partial class ServerTaskView
{
    public ServerTaskView()
    {

    }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public SignalService SignalService { get; set; }

    public IFutureList<SchedulerInfoWithLogViewModel> SchedulerInfos { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        SchedulerInfos = new FutureList<SchedulerInfoWithLogViewModel>(
            async () => ServerErrorManager.Eval(await HttpService.SchedulerApiAccess.GetSchedulerInfos())
                .With(e => e.Select(f => new SchedulerInfoWithLogViewModel(f)).ToArray()));
        await SchedulerInfos.Load();
        AddAsyncDisposable(await SignalService.LoggerEntryHub.BeginLogging(OnNewLog));
    }

    private Task OnNewLog(AppLoggerLogViewModel[] arg)
    {
        foreach (var appLoggerLogViewModel in arg.GroupBy(e => e.Key).Where(e => e.Key != null))
        {
            var taskNameParts = appLoggerLogViewModel.Key.Split('.');
            if (taskNameParts.Length != 3)
            {
                continue;
            }

            var taskName = taskNameParts[1];
            var scheduler = SchedulerInfos.FirstOrDefault(e => e.SchedulerInfo.TaskInfo.Name == taskName);
            foreach (var loggerLogViewModel in appLoggerLogViewModel)
            {
                scheduler?.Log.Add(loggerLogViewModel);
            }

        }
        Render();
        return Task.CompletedTask;
    }

    public async Task RunTask(string name)
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await HttpService.SchedulerApiAccess.RunTask(name));
            ServerErrorManager.DisplayStatus();
        }
    }
}

public class SchedulerInfoWithLogViewModel
{
    public SchedulerInfoWithLogViewModel(SchedulerInfo schedulerInfo)
    {
        SchedulerInfo = schedulerInfo;
        Log = new RingBuffer<AppLoggerLogViewModel>(50, true);
    }

    public SchedulerInfo SchedulerInfo { get; }

    public RingBuffer<AppLoggerLogViewModel> Log { get; set; }
}