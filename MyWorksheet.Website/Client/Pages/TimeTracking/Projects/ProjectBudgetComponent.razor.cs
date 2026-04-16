using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public partial class ProjectBudgetComponent : NavigationPageBase
{
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// When false (default, project view): shows only the current user's personal budget.
    /// When true (org/manager view): shows all budgets in a table with add/edit/delete.
    /// </summary>
    [Parameter]
    public bool ShowAllBudgets { get; set; } = false;

    [Inject]
    public HttpService HttpService { get; set; }
    
    [Inject] 
    public CurrentUserStore CurrentUserStore { get; set; }

    public List<ProjectBudgetViewModel> Budgets { get; set; }

    /// <summary>Personal budget for the current user (used when ShowAllBudgets == false).</summary>
    public ProjectBudgetViewModel PersonalBudget { get; set; }

    public bool ShowAddForm { get; set; }
    public CreateProjectBudgetViewModel NewBudget { get; set; }
    public AccountApiUserGetInfo NewBudgetUser { get; set; }
    public UpdateProjectBudgetViewModel EditingBudget { get; set; }
    public AccountApiUserGetInfo EditingBudgetUser { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await RefreshBudgets();
    }

    private async Task RefreshBudgets()
    {
        NewBudget = new CreateProjectBudgetViewModel { IdProject = ProjectId };
        NewBudgetUser = null;
        ShowAddForm = false;
        EditingBudget = null;
        EditingBudgetUser = null;

        if (ShowAllBudgets)
        {
            var result = ServerErrorManager.Eval(await HttpService.ProjectBudgetApiAccess.GetProjectBudgets(ProjectId));
            Budgets = result.Success ? result.Object?.ToList() ?? new List<ProjectBudgetViewModel>() : new List<ProjectBudgetViewModel>();
            PersonalBudget = null;
        }
        else
        {
            var result = ServerErrorManager.Eval(await HttpService.ProjectBudgetApiAccess.GetUsersBudget(ProjectId));
            PersonalBudget = result.Success ? result.Object : null;
            Budgets = PersonalBudget != null
                ? new List<ProjectBudgetViewModel> { PersonalBudget }
                : new List<ProjectBudgetViewModel>();
        }
    }

    public async Task CreateBudget()
    {
        ServerErrorManager.ServerErrors.Clear();
        using (WaiterService.WhenDisposed())
        {
            if (ShowAllBudgets)
            {
                NewBudget.IdAppUser = NewBudgetUser?.UserID;
            }
            else
            {
                NewBudget.IdAppUser = CurrentUserStore.CurrentToken.UserData.UserInfo.UserID;
            }
            // In personal mode IdAppUser stays null (the server resolves current user's budget)

            var result = ServerErrorManager.Eval(await HttpService.ProjectBudgetApiAccess.CreateBudget(NewBudget));
            if (result.Success)
            {
                if (ShowAllBudgets)
                {
                    Budgets.Add(result.Object);
                }
                else
                {
                    PersonalBudget = result.Object;
                    Budgets = new List<ProjectBudgetViewModel> { PersonalBudget };
                }

                NewBudget = new CreateProjectBudgetViewModel { IdProject = ProjectId };
                NewBudgetUser = null;
                ShowAddForm = false;
                WaiterService.DisplayOk();
            }
        }
    }

    public void BeginEdit(ProjectBudgetViewModel budget)
    {
        EditingBudget = new UpdateProjectBudgetViewModel
        {
            ProjectBudgetId = budget.ProjectBudgetId,
            IdProject = budget.IdProject,
            IdAppUser = budget.IdAppUser,
            TotalBudget = budget.TotalBudget,
            TotalTimeBudget = budget.TotalTimeBudget,
            ValidFrom = budget.ValidFrom,
            Deadline = budget.Deadline,
            AllowOverbooking = budget.AllowOverbooking,
            RowVersion = budget.RowVersion,
        };
        EditingBudgetUser = null;
    }

    public void CancelEdit()
    {
        EditingBudget = null;
        EditingBudgetUser = null;
    }

    public async Task SaveEditBudget()
    {
        ServerErrorManager.ServerErrors.Clear();
        using (WaiterService.WhenDisposed())
        {
            if (ShowAllBudgets && EditingBudgetUser != null)
            {
                EditingBudget.IdAppUser = EditingBudgetUser.UserID;
            }

            var result = ServerErrorManager.Eval(await HttpService.ProjectBudgetApiAccess.UpdateBudget(EditingBudget));
            if (result.Success)
            {
                if (ShowAllBudgets)
                {
                    var idx = Budgets.FindIndex(b => b.ProjectBudgetId == result.Object.ProjectBudgetId);
                    if (idx >= 0)
                    {
                        Budgets[idx] = result.Object;
                    }
                }
                else
                {
                    PersonalBudget = result.Object;
                    Budgets = new List<ProjectBudgetViewModel> { PersonalBudget };
                }

                EditingBudget = null;
                EditingBudgetUser = null;
                WaiterService.DisplayOk();
            }
        }
    }

    public async Task DeleteBudget(ProjectBudgetViewModel budget)
    {
        ServerErrorManager.ServerErrors.Clear();
        using (WaiterService.WhenDisposed())
        {
            var result = ServerErrorManager.Eval(await HttpService.ProjectBudgetApiAccess.DeleteBudget(budget.ProjectBudgetId));
            if (result.Success)
            {
                Budgets.Remove(budget);
                if (!ShowAllBudgets)
                {
                    PersonalBudget = null;
                }
                WaiterService.DisplayOk();
            }
        }
    }

    /// <summary>
    /// Returns a display string for a budget's assigned user.
    /// Since we don't cache user info locally, fall back to the raw ID for now.
    /// </summary>
    public string GetUserDisplay(ProjectBudgetViewModel budget)
    {
        return budget.IdAppUser?.ToString() ?? string.Empty;
    }

    public async Task<IEnumerable<AccountApiUserGetInfo>> SearchUsers(string search)
    {
        var result = await HttpService.AccountApiAccess.GetAssosiatedUsers(1, 10, search);
        return result.Object?.CurrentPageItems ?? Enumerable.Empty<AccountApiUserGetInfo>();
    }
}
