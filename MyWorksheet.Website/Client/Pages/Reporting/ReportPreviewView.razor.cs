using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Reporting;

public partial class ReportPreviewView
{
    [Parameter]
    public Guid Id { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    public ValueBag Values { get; set; }
    public IObjectSchemaInfo TemplateSchema { get; set; }

    public NEngineTemplateLookup TemplateData { get; set; }

    public Guid LastScheduledReport { get; set; }

    public string TemplateUrl { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        var apiResult = ServerErrorManager.Eval(await HttpService.TemplateManagementApiAccess.GetTemplate(Id));
        if (!apiResult.Success)
        {
            return;
        }
        TemplateData = apiResult.Object;
        Values = new ValueBag();
        var tokenizeTemplate = await HttpService.ReportManagementApiAccess.GetSchemaForRds(TemplateData.UsedDataSource);
        TemplateSchema = tokenizeTemplate.Object.ArgumentSchema;

        await CurrentUserStore
            .WhenChanged()
            .UserIsAuthenticated(() =>
            {
                ChangeTrackingService.RegisterTracking("PreviewReport", OnPreviewReportChanged);
                ChangeTrackingService.RegisterTracking(typeof(NEngineTemplateLookup), OnReportChanged);
                OnNextRender(LoadReport);
            }).Invoke();
    }

    private async void OnReportChanged(EntityChangedEventArguments eventArguments)
    {
        if (!eventArguments.Ids.Contains(Id))
        {
            return;
        }

        using (WaiterService.WhenDisposed())
        {
            await LoadReport();
        }
    }

    private async void OnPreviewReportChanged(EntityChangedEventArguments eventArguments)
    {
        //Console.WriteLine($"Report Changed: {type}-{id}");
        if (eventArguments.Ids.Contains(TemplateData?.NEngineTemplateId ?? Guid.Empty) && eventArguments.ChangeEventTypes == ChangeEventTypes.Removed)
        {
            await RefreshReport();
        }
    }

    public async Task RefreshReport()
    {
        using (WaiterService.WhenDisposed())
        {
            var token = await HttpService.TemplateManagementApiAccess.IssueReportDownloadToken(TemplateData.NEngineTemplateId);
            TemplateUrl = HttpService.TemplateManagementApiAccess.DownloadPreviewUrl(TemplateData.NEngineTemplateId, token.Object);
            StateHasChanged();
        }
    }

    private async Task LoadReport()
    {
        TemplateUrl = null;
        StateHasChanged();
        using (WaiterService.WhenDisposed())
        {
            var scheduleReport = await HttpService.ReportManagementApiAccess.ScheduleReport(new ScheduleReportModel()
            {
                Arguments = Values.Values,
                Preview = true,
                ParameterValues = Array.Empty<ReportingExecutionParameterValue>(),
                TemplateId = Id,
                StorageProvider = null
            });
            if (scheduleReport.Success)
            {
                LastScheduledReport = scheduleReport.Object;
            }
        }
    }
}