using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Shared.Dialog;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Client.Util.View.List;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.Organisations;

public partial class OrganizationEditView
{
    private bool _displayRemoved;
    private bool _displayAdded;

    public OrganizationEditView()
    {
        EditNewOrganization = new EditOrganizationMapViewModel();

    }

    [Parameter] public Guid? Id { get; set; }

    [Inject] public HttpService HttpService { get; set; }
    [Inject] public OrganizationService OrganizationService { get; set; }
    [Inject] public CurrentUserStore CurrentUserStore { get; set; }
    public bool DisplayNewRelation { get; set; }
    public EditOrganizationMapViewModel EditNewOrganization { get; set; }
    public EditOrganizationViewModel EditOrganization { get; set; }
    public IList<GetProjectModel> OrgProjects { get; set; }

    public bool DisplayAdded
    {
        get { return _displayAdded; }
        set
        {
            _displayAdded = value;
            if (value)
            {
                DisplayRemoved = false;
            }
        }
    }

    public bool DisplayRemoved
    {
        get { return _displayRemoved; }
        set
        {
            _displayRemoved = value;
            if (value)
            {
                DisplayAdded = false;
            }
        }
    }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("OrgOverview/Title"));
        var model = new EditOrganizationViewModel();
        if (!Id.HasValue)
        {
            model.AddressModel = new EntityState<AddressModel>(new AddressModel(), EntityListState.Added);
            model.OrgModel = new EntityState<OrganizationSelectionViewModel>(new OrganizationSelectionViewModel(),
                EntityListState.Added);
            await SetTitleAsync(new LocalizableString("Organization/Title", new LocalizableString("Common/New")));
        }
        else
        {
            var orgEntityApiState = await OrganizationService.Organizations.Cache.Find(Id.Value);
            if (orgEntityApiState == null)
            {
                return;
            }

            model.OrgModel = orgEntityApiState;
            model.AddressModel =
                ServerErrorManager.EvalAndUnbox(await HttpService.AddressApiAccess.OrganizationAddress(Id.Value));
            model.UsersInOrg = new PagedList<OrganizationMapViewModel>(
                (list) => ServerErrorManager.Eval(HttpService.OrganizationApiAccess
                    .GetUsersInOrg(Id.Value, list.Page, list.PageSize).AsTask()), WaiterService);
            model.UsersInOrg.PageSize = 6;
            if (orgEntityApiState.IdParentOrganisation.HasValue)
            {
                model.ParentOrganization =
                    await OrganizationService.Organizations.Cache.Find(orgEntityApiState.IdParentOrganisation
                        .Value);
            }

            TrackWhen()
                .Changed(model.UsersInOrg)
                .ThenRefresh(this);
            await model.UsersInOrg.SearchAsync();
            await SetTitleAsync(new LocalizableString("Organization/Title", model.OrgModel?.Entity?.Name));

            var projectsResult = ServerErrorManager.Eval(
                await HttpService.ProjectApiAccess.GetProjectsByOrganisation(Id.Value));
            OrgProjects = projectsResult.Success ? projectsResult.Object?.ToList() : new List<GetProjectModel>();
        }

        model.OrganizationContext = new EditContext(model.OrgModel.Entity);
        model.AddressContext = new EditContext(model.AddressModel.Entity);

        WhenChanged(model.OrgModel.Entity)
            .Changed(model.AddressModel.Entity)
            .Changed(OrganizationService.Roles)
            .ThenRefresh(this);

        EditOrganization = model;

        WhenChanged(OrganizationService.Roles).Then(() =>
        {
            EditNewOrganization = new EditOrganizationMapViewModel()
            {
                OrganizationMapViewModel =
                {
                    IdRelation = OrganizationService.Roles.First().Id
                }
            };
        });
    }

    public void Delete()
    {
        var msgBox =
            MessageBoxDialogViewModel.YesNo(new LocalizableString("Common.Delete/ConfirmDelete.Title"),
                new LocalizableString("Common.Delete/ConfirmDelete.Body",
                    new LocalizableString("Entity/Organization"),
                    ""));

        DialogService.ShowMessageBox(msgBox)
            .Closed(async () =>
            {
                if (msgBox.Result is not true)
                {
                    return;
                }

                var apiResult = ServerErrorManager.Eval(await HttpService.OrganizationApiAccess.Delete(Id.Value));

                if (apiResult.Success)
                {
                    ModuleService.NavigateTo("/Organizations");
                }
            });
    }

    public async Task Save()
    {
        var a = EditOrganization.AddressContext.Validate();
        var b = EditOrganization.OrganizationContext.Validate();
        if (a == false || b == false)
        {
            return;
        }

        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            if (!Id.HasValue)
            {
                var valueTask = ServerErrorManager.Eval(await HttpService.OrganizationApiAccess.Create(
                    new OrganizationGroupViewModel()
                    {
                        Address = EditOrganization.AddressModel.Entity,
                        OrganizationViewModel = EditOrganization.OrgModel.Entity,
                        Users = EditOrganization.ModifiedEntries.Values
                            .Select(f => new ApiEntityState<UpdateOrganizationMapViewModel>(f.Entity, f.ListState))
                            .ToArray()
                    }));
                if (valueTask.Success)
                {
                    ModuleService.NavigateTo("/Organization/" + valueTask.Object.OrganisationId);
                    await LoadDataAsync();
                    return;
                }
            }
            else
            {
                var valueTask = ServerErrorManager.Eval(await HttpService.OrganizationApiAccess.Update(
                    new OrganizationGroupViewModel()
                    {
                        Address = EditOrganization.AddressModel.IsObjectDirty
                            ? EditOrganization.AddressModel.Entity
                            : null,
                        OrganizationViewModel = EditOrganization.OrgModel.IsObjectDirty
                            ? EditOrganization.OrgModel.Entity
                            : null,
                        Users = EditOrganization.ModifiedEntries.Values
                            .Select(f => new ApiEntityState<UpdateOrganizationMapViewModel>(f.Entity, f.ListState))
                            .ToArray()
                    }, Id.Value));
                ServerErrorManager.DisplayStatus();
                if (valueTask.Success)
                {
                    await LoadDataAsync();
                }
            }
        }
    }

    public void MarkUserAssosiationAsDeleted(OrganizationMapViewModel map)
    {
        if (EditOrganization.ModifiedEntries.ContainsKey(GetMapKey(map)))
        {
            EditOrganization.ModifiedEntries.Remove(GetMapKey(map));
        }
        else
        {
            EditOrganization.ModifiedEntries[GetMapKey(map)] =
                new EntityState<OrganizationMapViewModel>(map, EntityListState.Deleted);
        }
    }

    public void AddUserAssosiation(OrganizationMapViewModel map)
    {
        EditOrganization.ModifiedEntries[GetMapKey(map)] =
            new EntityState<OrganizationMapViewModel>(map, EntityListState.Added);
    }

    private static string GetMapKey(OrganizationMapViewModel map)
    {
        return map.IdAppUser + "_" + map.IdRelation;
    }

    private async Task<IEnumerable<AccountApiUserGetInfo>> SearchAssosiatedUsers(string arg)
    {
        return (await HttpService.AccountApiAccess.GetAssosiatedUsers(1, 10, arg)).UnpackOrThrow().Object
            ?.CurrentPageItems;
    }

    private void AddUserAssosiationFromEdit()
    {
        AddUserAssosiation(EditNewOrganization.OrganizationMapViewModel);
        EditNewOrganization = new EditOrganizationMapViewModel()
        {
            OrganizationMapViewModel =
            {
                IdRelation = OrganizationService.Roles.First().Id
            }
        };
        DisplayNewRelation = false;
    }
}

public class EditOrganizationViewModel : ViewModelBase
{
    public EditOrganizationViewModel()
    {
        ModifiedEntries = new Dictionary<string, EntityState<OrganizationMapViewModel>>();
    }

    public EntityState<OrganizationSelectionViewModel> OrgModel { get; set; }
    public EntityState<AddressModel> AddressModel { get; set; }
    public PagedList<OrganizationMapViewModel> UsersInOrg { get; set; }
    public IDictionary<string, EntityState<OrganizationMapViewModel>> ModifiedEntries { get; set; }

    private OrganizationViewModel _parentOrganization;

    public OrganizationViewModel ParentOrganization
    {
        get { return _parentOrganization; }
        set
        {
            if (SetProperty(ref _parentOrganization, value))
            {
                OrgModel.Entity.IdParentOrganisation = value?.OrganisationId;
            }
        }
    }

    public EditContext OrganizationContext { get; set; }
    public EditContext AddressContext { get; set; }
}

public class EditOrganizationMapViewModel : ViewModelBase
{
    public EditOrganizationMapViewModel()
    {
        OrganizationMapViewModel = new OrganizationMapViewModel();
        OrganizationMapViewModel.IdRelation = Guid.Empty;
    }

    public OrganizationMapViewModel OrganizationMapViewModel { get; set; }

    public AccountApiUserGetInfo SelectedUser
    {
        get { return OrganizationMapViewModel.AppUser; }
        set
        {
            OrganizationMapViewModel.AppUser = value;
            if (value != null)
            {
                OrganizationMapViewModel.IdAppUser = value.UserID;
            }
        }
    }
}