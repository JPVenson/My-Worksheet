using System.Linq;
using System;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Budget;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[RevokableAuthorize]
[Route("api/ProjectBudgetApi")]
public class ProjectBudgetApiControllerBase : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IBudgetService _budgetService;

    public ProjectBudgetApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory, IBudgetService budgetService)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
        _budgetService = budgetService;
    }

    [NonAction]
    private string[] GetRolesInOrg(Guid organisationId)
    {
        return GetRolesInOrg(organisationId, User.GetUserId());
    }

    [NonAction]
    private string[] GetRolesInOrg(Guid organisationId, Guid appUserId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.OrganisationUserMaps.Where(e => e.IdAppUser == appUserId && e.IdOrganisation == organisationId)
            .ToArray()
            .Select(e => UserToOrgRoles.Yield().FirstOrDefault(f => f.Id == e.IdRelation)?.Name)
            .ToArray();
    }

    [HttpGet]
    [Route("GetUsersBudget")]
    public IActionResult GetUsersBudget(Guid projectId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<ProjectBudgetViewModel>(
            _budgetService.BudgetForUserInProject(db, projectId, User.GetUserId())));
    }

    [HttpGet]
    [Route("GetProjectBudgets")]
    public IActionResult GetBudgetsForProject(Guid projectId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var project = db.Projects.Find(projectId);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);

            if (!GetRolesInOrg(org.OrganisationId).Contains(UserToOrgRoles.ProjectManager.Name))
            {
                return BadRequest("Org/NotPartOfOrg".AsTranslation());
            }
        }

        var budgets = db.ProjectBudgets.Where(e => e.IdProject == projectId)
            .ToArray();
        return Data(_mapper.ViewModelMapper.Map<ProjectBudgetViewModel[]>(budgets));
    }

    [HttpPost]
    [Route("CreateBudget")]
    public IActionResult CreateBudget([FromBody] CreateProjectBudgetViewModel budget)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var project = db.Projects.Find(budget.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (org.IsDeleted)
            {
                return Unauthorized("Common/InvalidId".AsTranslation());
            }
            if (!org.IsActive)
            {
                return BadRequest("You cannot create a Budget for an inactive Organistaion");
            }

            if (!GetRolesInOrg(org.OrganisationId).Contains(UserToOrgRoles.ProjectManager.Name))
            {
                return BadRequest("Org/NotPartOfOrg".AsTranslation());
            }

            if (budget.IdAppUser.HasValue && !GetRolesInOrg(org.OrganisationId, budget.IdAppUser.Value).Any())
            {
                return BadRequest("The selected user is not part of the Organisation");
            }
        }

        var hasBudget = db.ProjectBudgets.Where(e => e.IdProject == budget.IdProject && e.IdAppUser == budget.IdAppUser)
            .FirstOrDefault();

        if (hasBudget != null)
        {
            return BadRequest("There is an existing Budget for this Project and user");
        }

        var budgetEntity = _mapper.ViewModelMapper.Map<ProjectBudget>(budget);        
        _budgetService.ReevaluateBudget(db, budgetEntity);
        budgetEntity.RowVersion = Guid.NewGuid().ToByteArray();
        db.Add(budgetEntity);
        db.SaveChanges();
        //AccessElement<BudgetHubInfo>.Instance.SendProjectBudgetChanged(budgetEntity.IdProject, budgetEntity.ProjectBudgetId);
        return Data(_mapper.ViewModelMapper.Map<ProjectBudgetViewModel>(budgetEntity));
    }

    [HttpPost]
    [Route("UpdateBudget")]
    public IActionResult UpdateBudget([FromBody] UpdateProjectBudgetViewModel budgetUpdate)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var budget = db.ProjectBudgets.Find(budgetUpdate.ProjectBudgetId);
        if (budget == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var project = db.Projects.Find(budget.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (!org.IsActive)
            {
                return BadRequest("You cannot create a Budget for an inactive Organistaion");
            }

            if (!GetRolesInOrg(org.OrganisationId).Contains(UserToOrgRoles.ProjectManager.Name))
            {
                return BadRequest("Org/NotPartOfOrg".AsTranslation());
            }
        }

        var budgetEntity = _mapper.ViewModelMapper.Map(budgetUpdate, budget);
        _budgetService.ReevaluateBudget(db, budgetEntity);
        budgetEntity.RowVersion = Guid.NewGuid().ToByteArray();
        db.SaveChanges();

        //AccessElement<BudgetHubInfo>.Instance.SendProjectBudgetChanged(budgetEntity.IdProject, budgetEntity.ProjectBudgetId);
        return Data(_mapper.ViewModelMapper.Map<ProjectBudgetViewModel>(budgetEntity));
    }

    [HttpPost]
    [Route("DeleteBudget")]
    public IActionResult Delete(Guid budgetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var budget = db.ProjectBudgets.Find(budgetId);
        if (budget == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var project = db.Projects.Find(budget.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (!org.IsActive)
            {
                return BadRequest("You cannot create a Budget for an inactive Organistaion");
            }

            if (!GetRolesInOrg(org.OrganisationId).Contains(UserToOrgRoles.ProjectManager.Name))
            {
                return BadRequest("Org/NotPartOfOrg".AsTranslation());
            }
        }
        db.ProjectBudgets.Remove(budget);
        db.SaveChanges();
        _budgetService.Add(db, budget.IdProject, User.GetUserId(), (int)budget.TimeConsumed, true);

        return Data();
    }
}