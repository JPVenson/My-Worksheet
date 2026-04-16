using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.MailAccount;

public partial class MailAccountListView
{
    public MailAccountListView()
    {

    }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ICacheRepository<EMailAccountViewModel> MailAccounts { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await MailAccounts.Cache.LoadAll();
    }
}