using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.MailAccount;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.MailAccount;

public partial class MailAccountEditView
{
    [Parameter]
    public Guid? Id { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public ICacheRepository<EMailAccountViewModel> MailAccounts { get; set; }
    [Inject]
    public MailAccountService MailAccountService { get; set; }

    public IList<EMailProtcol> EMailProtcols { get; set; }

    public EntityState<EMailAccountViewModel> MailAccount { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        EMailProtcols = MailAccountService.GetProtocols().ToList();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/MailAccounts"));

        if (!Id.HasValue)
        {
            MailAccount = new EntityState<EMailAccountViewModel>(new EMailAccountViewModel()
            {

            }, EntityListState.Added);
            await SetTitleAsync(new LocalizableString("MailAccount/Title", new LocalizableString("Common/New")));
        }
        else
        {
            MailAccount = await MailAccounts.Cache.Find(Id.Value);
            await SetTitleAsync(new LocalizableString("MailAccount/Title", MailAccount.Entity.Name));
        }
    }

    public async Task Save()
    {
        if (!MailAccount.IsObjectDirty)
        {
            return;
        }

        using (WaiterService.WhenDisposed())
        {
            if (MailAccount.ListState == EntityListState.Added)
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.EMailAccountApiAccess.Create(MailAccount.Entity));
                ServerErrorManager.DisplayStatus();
                if (apiResult.Success)
                {
                    ModuleService.NavigateTo("/MailAccount/" + apiResult.Object.MailAccountId);
                    return;
                }
            }
            else
            {

                var apiResult = ServerErrorManager.Eval(await HttpService.EMailAccountApiAccess.Update(MailAccount.Entity, MailAccount.Entity.MailAccountId));
                ServerErrorManager.DisplayStatus();
            }
        }
    }

    public async Task Delete()
    {
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.EMailAccountApiAccess.Create(MailAccount.Entity));
            ServerErrorManager.DisplayStatus();
            if (apiResult.Success)
            {
                ModuleService.NavigateTo("/MailAccounts");
                return;
            }
        }
    }
}