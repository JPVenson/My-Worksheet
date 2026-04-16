using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Pages.Shared.Dialog;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Webhook;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Client.Util.View.List;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.Hooks;

public partial class HttpHooksEditView
{
    public HttpHooksEditView()
    {

    }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public WebhookService WebhookService { get; set; }

    [Parameter]
    public Guid? Id { get; set; }

    public OutgoingWebhookCaseModel SelectedWebhookType { get; set; }

    public EntityState<OutgoingWebhookModelGet> Webhook { get; set; }
    public PagedList<OutgoingWebhookActionLogModel> LogEntries { get; set; }
    public IObjectSchemaInfo TypeSchema { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Webhooks"));
        if (Id is null || !Id.HasValue)
        {
            Webhook = new EntityState<OutgoingWebhookModelGet>(new OutgoingWebhookModelGet()
            {
                Name = "New Webhook",
                CallingUrl = "https://",
                IsActive = true
            }, EntityListState.Added);
            await SetTitleAsync(new LocalizableString("Links/Webhook", new LocalizableString("Common/New")));

            await WebhookService.WebhookTypes.WhenLoadedOnceAsync(async () =>
            {
                SelectedWebhookType = WebhookService.WebhookTypes.FirstOrDefault();
                Webhook.Entity.IdOutgoingWebhookCase = SelectedWebhookType.OutgoingWebhookCaseId;
                await LoadSchema();
            });
        }
        else
        {
            Webhook = ServerErrorManager.EvalAndUnbox(await HttpService.WebhookApiAccess.OutgoingApiWebhookApiAccess.Get(Id.Value).AsTask());
            if (Webhook == null)
            {
                return;
            }

            WebhookService.WebhookTypes.WhenLoadedOnce(() =>
            {
                SelectedWebhookType = WebhookService.WebhookTypes.FirstOrDefault(e =>
                    e.OutgoingWebhookCaseId == Webhook.Entity.IdOutgoingWebhookCase);
            });

            LogEntries = new PagedList<OutgoingWebhookActionLogModel>(
                (page) => HttpService.WebhookApiAccess.OutgoingApiWebhookApiAccess.GetLog(Id.Value, page.Page, page.PageSize).AsTask(), WaiterService);
            await SetTitleAsync(new LocalizableString("Links/Webhook", Webhook.Entity.NumberRangeEntry));
            await LoadSchema();
        }
    }

    public async Task LoadSchema()
    {
        using (WaiterService.WhenDisposed())
        {
            TypeSchema = ServerErrorManager.EvalAndUnbox(
                await HttpService
                    .WebhookApiAccess
                    .OutgoingApiWebhookApiAccess
                    .GetTestData(Webhook.Entity.IdOutgoingWebhookCase));
        }
    }

    public async Task Save()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            if (Webhook.ListState == EntityListState.Added)
            {
                var valueTask = ServerErrorManager.Eval(await HttpService.WebhookApiAccess.OutgoingApiWebhookApiAccess.Create(Webhook.Entity));
                if (valueTask.Success)
                {
                    NavigationService.NavigateTo("/Settings/Webhook/" + valueTask.Object.OutgoingWebhookId, true);
                    return;
                }
            }
            else
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.WebhookApiAccess.OutgoingApiWebhookApiAccess.Update(Webhook.Entity));
                if (apiResult.Success)
                {
                    Webhook = apiResult.Object;
                }
            }
            ServerErrorManager.DisplayStatus();
        }
    }

    public void Delete()
    {
        var messageBoxDialogViewModel = MessageBoxDialogViewModel.YesNo("HttpHook/ConfirmClose.Title", "HttpHook/ConfirmClose.Body");
        DialogService.Show("MessageBox", messageBoxDialogViewModel)
            .Closed(async () =>
            {
                if (messageBoxDialogViewModel.Result is bool val && val == true)
                {
                    using (WaiterService.WhenDisposed())
                    {
                        var apiResult = ServerErrorManager.Eval(await HttpService
                            .WebhookApiAccess
                            .OutgoingApiWebhookApiAccess
                            .Delete(Webhook.Entity.OutgoingWebhookId));
                        if (apiResult.Success)
                        {
                            ModuleService.NavigateTo("/Settings/Webhooks");
                        }
                    }
                }
            });
    }

    public void ShowSchema()
    {
        DialogService.Show("ShowHttpHookStructure");
    }
}