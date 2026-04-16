using System;
using System.Linq;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Website.Server.Models;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Budget;

[ScopedService(typeof(IBudgetService))]
public class BudgetService : IBudgetService
{
    public void ReevaluateBudget(MyworksheetContext db, ProjectBudget budget)
    {
        var worktimeFromStartDate = 0;

        var budgetStartDate = budget.ValidFrom ?? DateTimeOffset.MinValue;

        if (budget.IdAppUser.HasValue)
        {
            worktimeFromStartDate = db.WorksheetItems
                .Where(f => f.IdCreator == budget.IdAppUser.Value)
                .Where(e => db.Worksheets
                    .Where(f => f.IdProject == budget.IdProject)
                    .Where(f => f.IdCreator == budget.IdAppUser.Value)
                    .Select(e => e.WorksheetId)
                    .Contains(e.IdWorksheet))
                .Where(f => f.DateOfAction >= budgetStartDate.Date)
                .Select(e => e.ToTime - e.FromTime)
                .Sum();
        }
        else
        {
            worktimeFromStartDate = db.WorksheetItems
                     .Where(f => f.DateOfAction >= budgetStartDate.Date)
                     .Where(f => db.Worksheets
                        .Where(f => f.IdProject == budget.IdProject)
                        .Where(f => !db.ProjectBudgets
                            .Where(f => f.IdProject == budget.IdProject)
                            .Where(f => f.IdAppUser != null)
                            .Select(e => e.IdAppUser.Value)
                            .Contains(f.IdCreator))
                        .Select(e => e.WorksheetId)
                        .Contains(f.IdWorksheet))
                     .Select(e => e.ToTime - e.FromTime)
                     .Sum();
        }

        budget.TimeConsumed = worktimeFromStartDate;
    }

    public ProjectBudget BudgetForUserInProject(MyworksheetContext db, Guid idProject, Guid forUser)
    {
        var budget = db.ProjectBudgets
            .Where(f => f.IdProject == idProject)
            .Where(f => f.IdAppUser == forUser)
            .FirstOrDefault();

        if (budget != null)
        {
            return budget;
        }

        budget = db.ProjectBudgets
            .Where(f => f.IdProject == idProject && f.IdAppUser == null)
            .FirstOrDefault();

        return budget;
    }

    public QuestionableBoolean Add(MyworksheetContext db, Guid projectId, Guid forUser, int time, bool overwrite = false)
    {
        if (!overwrite)
        {
            var canAdd = ExceedsBudget(db, projectId, forUser, time);

            if (!canAdd)
            {
                return canAdd;
            }
        }

        var budget = BudgetForUserInProject(db, projectId, forUser);

        if (budget == null)
        {
            return true;
        }

        db.ProjectBudgets.Where(e => e.ProjectBudgetId == budget.ProjectBudgetId)
            .ExecuteUpdate(e => e.SetProperty(f => f.TimeConsumed, f => f.TimeConsumed + time));

        //AccessElement<BudgetHubInfo>.Instance.SendProjectBudgetChanged(budget.IdProject, budget.ProjectBudgetId);

        return true;
    }

    public QuestionableBoolean Substract(MyworksheetContext db, Guid projectId, Guid forUser, int time, bool overwrite = false)
    {
        if (!overwrite)
        {
            var canAdd = ExceedsBudget(db, projectId, forUser, time * -1);

            if (!canAdd)
            {
                return canAdd;
            }
        }

        var budget = BudgetForUserInProject(db, projectId, forUser);

        if (budget == null)
        {
            return true;
        }

        db.ProjectBudgets.Where(e => e.ProjectBudgetId == budget.ProjectBudgetId)
            .ExecuteUpdate(e => e.SetProperty(f => f.TimeConsumed, f => f.TimeConsumed - time));
        //AccessElement<BudgetHubInfo>.Instance.SendProjectBudgetChanged(budget.IdProject, budget.ProjectBudgetId);

        return true;
    }

    public QuestionableBoolean ExceedsBudget(MyworksheetContext db, Guid idProject, Guid forUser, int time)
    {
        var budget = BudgetForUserInProject(db, idProject, forUser);

        if (budget == null)
        {
            return true.Because("No Budget for ether user or project found");
        }

        if (budget.AllowOverbooking)
        {
            return true;
        }

        if (budget.Deadline.HasValue)
        {
            if (DateTime.UtcNow.Date > budget.Deadline.Value)
            {
                return false.Because($"The Deadline of {budget.Deadline.Value:D} has been reached");
            }
        }

        if (budget.TotalTimeBudget.HasValue)
        {
            if (budget.TotalTimeBudget.Value < budget.TimeConsumed + time)
            {
                return false.Because(
                    $"The time of {time} minutes would exceed the time budget of remaining {budget.TotalTimeBudget.Value - budget.TimeConsumed}");
            }
        }

        return true;


        //var otherUserBudgets = db.ProjectBudgets
        //	.Where
        //	.Column(f => f.IdProject).Is.EqualsTo(idProject)
        //	.And
        //	.Column(f => f.IdAppUser).Is.Not.Null
        //	.ToArray();

        //var hasProjectBudget = db.ProjectBudgets
        //	.Where
        //	.Column(f => f.IdProject).Is.EqualsTo(idProject)
        //	.And
        //	.Column(f => f.IdAppUser).Is.Null
        //	.FirstOrDefault();

        //int totalUsersBudget;
        //totalUsersBudget = otherUserBudgets.Select(e => e.TotalTimeBudget.GetValueOrDefault()).Sum();
        //if (forUser.HasValue)
        //{
        //	if (hasProjectBudget?.TotalTimeBudget != null)
        //	{
        //		totalUsersBudget += time.GetValueOrDefault();
        //		if (totalUsersBudget > hasProjectBudget.TotalTimeBudget.GetValueOrDefault())
        //		{
        //			return false.Because(
        //				"The new Total time budget would exceed the current time budget for the project");
        //		}
        //	}

        //	return true;
        //}

        //if (hasProjectBudget?.TotalTimeBudget != null)
        //{
        //	if (totalUsersBudget < time.GetValueOrDefault())
        //	{
        //		return false.Because(
        //			"The new Total time budget would exceed the current time budget for the project");
        //	}
        //}

        //return true;
    }
}