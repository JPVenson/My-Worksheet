using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View.List;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Assosiation;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class UserAssosciationsViewComponent
{
    public UserAssosciationsViewComponent()
    {

    }

    public FutureList<UserAssosiationModel> UserAssosiations { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        UserAssosiations = new FutureList<UserAssosiationModel>(() => HttpService.AccountAssociationUserApiAccess.GetUserAssociation().AsTask());
    }
}