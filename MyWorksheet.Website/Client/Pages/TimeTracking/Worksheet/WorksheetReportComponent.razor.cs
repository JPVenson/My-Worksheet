using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Pages.Home;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public partial class WorksheetReportComponent
{
    public WorksheetReportComponent()
    {
        Schema = new Dictionary<string, IObjectSchemaInfo>();
    }
    [Inject]
    public HttpService HttpService { get; set; }
    [Parameter]
    public WorksheetEditViewModel Model { get; set; }

    public IFutureList<NEngineTemplateLookup> Reports { get; set; }
    public Dictionary<string, IObjectSchemaInfo> Schema { get; set; }

    public NEngineTemplateLookup SelectedTemplate { get; set; }

    public IObjectSchemaInfo TemplateSchema { get; set; }
    public ValueBag TemplateValues { get; set; }

    public string TemplateUrl { get; set; }

    public async Task TemplateChanged()
    {
        if (SelectedTemplate is null)
        {
            return;
        }

        if (!Schema.TryGetValue(SelectedTemplate.UsedDataSource, out var schema))
        {
            schema = ServerErrorManager.EvalAndUnbox(await HttpService.ReportManagementApiAccess.GetSchemaForRds(SelectedTemplate.UsedDataSource)).ArgumentSchema;
            Schema[SelectedTemplate.UsedDataSource] = schema;
        }

        TemplateValues.Clear();
        TemplateSchema = JsonSchema.EmptyNotNull;
        OnContinuesRender(() =>
        {
            var idPaymentCondition = Model.Project.IdPaymentCondition;
            schema.Defaults["PaymentInfos"] = idPaymentCondition;
            TemplateSchema = schema;
            TemplateSchema.Schema.Remove("Worksheet");
            TemplateValues["Worksheet"] = Model.Worksheet.WorksheetId;
            TemplateValues["PaymentInfos"] = schema.Defaults["PaymentInfos"];
            Render();
        });

        Render();
    }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        AddDisposable(LayoutController.Modifier(e => e.FullHeightContent = true));
        Reports = new FutureList<NEngineTemplateLookup>(() => ServerErrorManager.Eval(HttpService.TemplateManagementApiAccess.GetTemplates("Worksheet").AsTask()));
        await Reports.Load();
        SelectedTemplate = Reports.FirstOrDefault();
        await TemplateChanged();


        AddDisposable(ChangeTrackingService.RegisterTracking("PreviewReport", OnPreviewReportChanged));
        AddDisposable(ChangeTrackingService.RegisterTracking(typeof(NEngineTemplateLookup), OnReportChanged));
        await LoadReport();
    }

    private async void OnReportChanged(EntityChangedEventArguments eventArguments)
    {
        if (!eventArguments.Ids.Contains(SelectedTemplate.NEngineTemplateId))
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
        if (eventArguments.Ids.Contains(SelectedTemplate.NEngineTemplateId) && eventArguments.ChangeEventTypes == ChangeEventTypes.Removed)
        {
            await RefreshReport();
        }
    }

    public async Task RefreshReport()
    {
        using (WaiterService.WhenDisposed())
        {
            var token = await HttpService.TemplateManagementApiAccess.IssueReportDownloadToken(SelectedTemplate.NEngineTemplateId);
            TemplateUrl = HttpService.TemplateManagementApiAccess.DownloadPreviewUrl(SelectedTemplate.NEngineTemplateId, token.Object);
            StateHasChanged();
        }
    }

    private async Task LoadReport()
    {
        ServerErrorManager.Clear();
        TemplateUrl = null;
        StateHasChanged();
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await HttpService.ReportManagementApiAccess.ScheduleReport(new ScheduleReportModel()
            {
                Arguments = TemplateValues.Values,
                Preview = true,
                ParameterValues = Array.Empty<ReportingExecutionParameterValue>(),
                TemplateId = SelectedTemplate.NEngineTemplateId,
                StorageProvider = null
            }));
        }
    }
}