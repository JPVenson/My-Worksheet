using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[ApiController]
[RevokableAuthorize]
[Route("api/OrganizationApi")]
public class OrganizationApiControllerBase : RestApiControllerBase<Organisation, OrganizationSelectionViewModel>
{
    public OrganizationApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory, mapper)
    {
    }

    [HttpGet]
    [Route("GetRoles")]
    public IActionResult GetRoles()
    {
        return Data(UserToOrgRoles.Yield());
    }

    [HttpGet]
    [Route("Get")]
    public IActionResult GetDetailed(Guid id)
    {
        var db = EntitiesFactory.CreateDbContext();
        var org = db.OrganisationUserMaps
            .Include(e => e.IdOrganisationNavigation)
            .Include(e => e.IdAppUserNavigation)
            .Where(e => e.IdOrganisation == id)
            .ToArray();
        if (!org.Any())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (org.Any(e => e.IdOrganisationNavigation.IsDeleted))
        {
            return Data(new OrganizationSelectionViewModel()
            {
                Name = "Deleted",
                IsActive = false,
                OrganisationId = id
            });
        }

        var orgMap = org.First();
        var orgVm = MapperService.ViewModelMapper.Map<OrganizationSelectionViewModel>(orgMap.IdOrganisationNavigation);
        orgVm.IdRelations = org.Select(f => f.IdRelation).ToArray();
        return Data(orgVm);
    }

    [HttpPost]
    [Route("UpdateAddress")]
    public IActionResult UpdateAddress(Guid organizationId, [FromBody] AddressModel addressModel)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        if (org.IsDeleted)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!org.IsActive)
        {
            return BadRequest("Organisation is inactive and cannot be changed");
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        Address address = null;
        address = MapperService.ViewModelMapper.Map<Address>(addressModel);
        address.DateOfCreation = DateTime.UtcNow;
        address.IdOrganisation = org.OrganisationId;
        db.Add(address);
        db.SaveChanges();
        db.Organisations
            .Where(e => e.OrganisationId == org.OrganisationId)
            .ExecuteUpdate(e => e.SetProperty(f => f.IdAddress, address.AddressId));
        db.SaveChanges();
        return Data(MapperService.ViewModelMapper.Map<AddressModel>(address));
    }


    [HttpPost]
    [Route("AddUserToOrg")]
    public IActionResult AddUserToOrg(Guid organizationId, Guid appUserId, Guid relationId)
    {
        var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        if (org.IsDeleted)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!org.IsActive)
        {
            return BadRequest("Organisation is inactive and cannot be changed");
        }
        var user = db.AppUsers.Find(appUserId);

        if (user == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var role = db.OrganisationRoleLookups.Find(relationId);

        if (role == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        var hasRelation = db.OrganisationUserMaps.FirstOrDefault(e => e.IdAppUser == appUserId && e.IdRelation == relationId && e.IdOrganisation == organizationId);

        if (hasRelation != null)
        {
            return BadRequest("This user does already exists with this assosiation to the org");
        }

        db.Add(new OrganisationUserMap
        {
            IdRelation = relationId,
            IdAppUser = appUserId,
            IdOrganisation = organizationId
        });
        db.SaveChanges();
        return Data();
    }

    [HttpPost]
    [Route("RemoveUserToOrg")]
    public IActionResult RemoveUserToOrg(Guid organizationId, Guid appUserId, Guid relationId)
    {
        var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        if (org.IsDeleted)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!org.IsActive)
        {
            return BadRequest("Organisation is inactive and cannot be changed");
        }

        var user = db.AppUsers.Find(appUserId);

        if (user == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var role = db.OrganisationRoleLookups.Find(relationId);

        if (role == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        if (relationId == UserToOrgRoles.Administrator.Id)
        {
            var isThereAnotherAdmin = db.OrganisationUserMaps
            .Any(f => f.IdAppUser != appUserId && f.IdRelation == UserToOrgRoles.Administrator.Id && f.IdOrganisation == organizationId);

            if (isThereAnotherAdmin)
            {
                return BadRequest("You cannot remove the last remaining administrator from this Organisation");
            }
        }

        db.OrganisationUserMaps.Where(e => e.IdAppUser == appUserId && e.IdRelation == relationId && e.IdOrganisation == organizationId)
        .ExecuteDelete();
        return Data();
    }

    [NonAction]
    private string[] GetRolesInOrg(Guid organizationId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return db.OrganisationUserMaps
            .Where(e => e.IdAppUser == User.GetUserId() && e.IdOrganisation == organizationId)
            .Select(f => f.IdRelation)
            .AsEnumerable()
            .Select(e => UserToOrgRoles.Yield().FirstOrDefault(f => f.Id == e)?.Name)
            .ToArray();
    }

    [HttpGet]
    [Route("GetUsersInOrg")]
    public IActionResult GetUsersInOrganization(Guid organizationId, int page, int pageSize)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        var userMapQuery =
            db.OrganisationUserMaps
            .Include(e => e.IdAppUserNavigation)
            .Where(e => e.IdOrganisation == organizationId)
            .OrderBy(e => e.IdOrganisation);

        var pagedUserMap = userMapQuery.ForPagedResult(page, pageSize);
        var result = MapperService.ViewModelMapper.Map<PageResultSet<OrganizationMapViewModel>>(pagedUserMap);
        var usersInResult = result.CurrentPageItems.Select(e => e.IdAppUser).ToArray();

        if (!usersInResult.Any())
        {
            return Data(result);
        }

        var mappedUsers = MapperService.ViewModelMapper.Map<AccountApiUserGetInfo[]>(pagedUserMap.CurrentPageItems.Select(f => f.IdAppUserNavigation));

        foreach (var organisationMapViewModel in result.CurrentPageItems)
        {
            organisationMapViewModel.AppUser =
                mappedUsers.FirstOrDefault(e => e.UserID == organisationMapViewModel.IdAppUser);
        }

        return Data(result);
    }

    [HttpGet]
    [Route("SearchUsersInOrg")]
    public IActionResult SearchUsersInOrganization(Guid organizationId, int page, int pageSize, string username = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        var userMap = db.UserOrganisationMappings
            .Where(f => f.Organisation.OrganisationUserMaps.Any(f => f.IdOrganisation == organizationId));

        if (!string.IsNullOrWhiteSpace(username))
        {
            userMap = userMap.Where(f => f.Username.Contains(username));
        }

        var pagedUsers = userMap.OrderBy(e => e.ContactName)
            .ForPagedResult(page, pageSize);

        return Data(MapperService.ViewModelMapper.Map<PageResultSet<AccountApiUserGetInfo>>(pagedUsers));
    }

    [HttpGet]
    [Route("DayOverview")]
    public IActionResult GetDayOverview(Guid organizationId, DateTimeOffset onDay)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        var projectsInOrg = db.Projects.Where(f => f.IdOrganisation == organizationId && f.Hidden == false)
            .ToArray();

        if (!projectsInOrg.Any())
        {
            return Data();
        }

        var worksheetsInProject = db.Worksheets
            .Where(e => e.StartTime < onDay.Date && (e.EndTime > onDay.Date || e.EndTime == null) && projectsInOrg.Select(f => f.ProjectId).Contains(e.IdProject))
            .ToArray();

        if (!worksheetsInProject.Any())
        {
            return Data();
        }

        var worksheetItemsToday = db.WorksheetItems
            .Where(e => e.DateOfAction == onDay.Date)
            .Where(e => worksheetsInProject.Select(e => e.WorksheetId).Contains(e.IdWorksheet))
            .ToArray();

        var assingedUsers = db.AppUsers.Where(e => worksheetsInProject.Select(e => e.IdCreator).Contains(e.AppUserId))
            .ToArray();

        return Data(new OrganisationDayOverview()
        {
            Projects = MapperService.ViewModelMapper.Map<GetProjectModel[]>(projectsInOrg),
            Worksheets = MapperService.ViewModelMapper.Map<WorksheetModel[]>(worksheetsInProject),
            WorksheetItems = MapperService.ViewModelMapper.Map<WorksheetItemModel[]>(worksheetItemsToday),
            AppUsers = MapperService.ViewModelMapper.Map<AccountApiUserGetInfo[]>(assingedUsers)
        });
    }

    [HttpGet]
    [Route("GetUserInOrg")]
    public IActionResult GetUserInOrganization(Guid organizationId, Guid userId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(organizationId);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        var userMap = db.OrganisationUserMaps
            .Where(e => e.IdOrganisation == organizationId)
            .Where(e => e.IdAppUser == userId)
            .FirstOrDefault();

        if (userMap == null)
        {
            return BadRequest("The user is not part of the Organisation");
        }

        return Data(MapperService.ViewModelMapper.Map<AccountApiUserGetInfo>(db.AppUsers.Find(userMap.IdAppUser)));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/GetForUser")]
    public IActionResult GetAssosiatedOrganizations(Guid userId,
        int page,
        int pageSize,
        bool includeInactives = false,
        string search = null,
        Guid? searchForRole = null)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var query = db.OrganisationUserMaps
            .Include(e => e.IdOrganisationNavigation)
            .Include(e => e.IdAppUserNavigation)
            .Where(e => e.IdAppUser == userId)
            .Where(e => e.IdOrganisationNavigation.IsDeleted == false);


        if (!includeInactives)
        {
            query = query.Where(e => e.IdOrganisationNavigation.IsActive == true);
        }
        if (searchForRole.HasValue)
        {
            query = query.Where(e => e.IdRelation == searchForRole.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.IdOrganisationNavigation.Name.Contains(search) || e.IdOrganisationNavigation.SharedId.Contains(search));
        }

        var queryResult = query
                .OrderBy(e => e.IdOrganisation)
                .Select(e => new
                {
                    IdRelation = e.IdRelation,
                    IsActive = e.IdOrganisationNavigation.IsActive,
                    OrganisationId = e.IdOrganisation,
                    Name = e.IdOrganisationNavigation.Name,
                    IdAddress = e.IdOrganisationNavigation.IdAddress,
                    SharedId = e.IdOrganisationNavigation.SharedId
                })
                .ToArray()
                .GroupBy(e => e.OrganisationId)
                .Select(e =>
                {
                    var first = e.First();
                    return new OrganizationSelectionViewModel
                    {
                        IdRelations = e.Select(f => f.IdRelation).ToArray(),
                        IsActive = first.IsActive,
                        OrganisationId = first.OrganisationId,
                        Name = first.Name,
                        IdAddress = first.IdAddress,
                        SharedId = first.SharedId
                    };
                }).ToArray();
        var result = new PageResultSet<OrganizationSelectionViewModel>()
        {
            CurrentPageItems = queryResult,
            CurrentPage = page,
            PageSize = pageSize
        };
        return Data(result);
    }

    public override IActionResult GetList()
    {
        return GetAllAssosiatedOrganisations(User.GetUserId(), true);
    }

    [RevokableAuthorize()]
    [HttpGet]
    [Route("GetAllForUser")]
    public IActionResult GetAllAssosiatedOrganisations(bool includeInactives = false,
        string search = null,
        Guid? searchForRole = null)
    {
        return GetAllAssosiatedOrganisations(User.GetUserId(), includeInactives, search, searchForRole);
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/GetAllForUser")]
    public IActionResult GetAllAssosiatedOrganisations(Guid userId,
        bool includeInactives = false,
        string search = null,
        Guid? searchForRole = null)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var query = db.OrganisationUserMaps
            .Include(e => e.IdOrganisationNavigation)
            .Include(e => e.IdAppUserNavigation)
            .Where(e => e.IdAppUser == userId)
            .Where(e => e.IdOrganisationNavigation.IsDeleted == false);

        if (!includeInactives)
        {
            query = query.Where(e => e.IdOrganisationNavigation.IsActive == true);
        }
        if (searchForRole.HasValue)
        {
            query = query.Where(e => e.IdRelation == searchForRole.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.IdOrganisationNavigation.Name.Contains(search) || e.IdOrganisationNavigation.SharedId.Contains(search));
        }

        return Data(query.OrderBy(e => e.IdOrganisation)
            .GroupBy(e => e.IdOrganisation)
            .ToArray()
            .Select(e =>
            {
                var orgMap = e.First();
                var orgSel = MapperService.ViewModelMapper.Map<OrganizationSelectionViewModel>(orgMap.IdOrganisationNavigation);
                orgSel.IdRelations = e.Select(f => f.IdRelation).ToArray();
                return orgSel;
            }).ToArray());
    }

    [HttpGet]
    [Route("GetForUser")]
    public IActionResult GetAssosiatedOrganizations(int page, int pageSize, bool includeInactives = false,
        string search = null, Guid? searchForRole = null)
    {
        return GetAssosiatedOrganizations(User.GetUserId(), page, pageSize, includeInactives, search, searchForRole);
    }

    [HttpGet]
    [Route("GetWorktimeForUsers")]
    public IActionResult GetWorktimeForUsers(Guid projectId, int page, int pageSize, string search = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var project = db.Projects.Find(projectId);

        if (project == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!project.IdOrganisation.HasValue)
        {
            return BadRequest("The Project is not Assigned to a Organisation");
        }

        var orgOfProject = db.Organisations.Find(project.IdOrganisation.Value);

        if (!GetRolesInOrg(orgOfProject.OrganisationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("You are not part of the Organisation in the Project");
        }

        var usersInProject = db.OrganisationWorksheets
            .Where(e => e.OrganisationId == orgOfProject.OrganisationId)
            .Select(e => new OrganisationWorksheet()
            {
                IdAppUser = e.IdAppUser,
                IsActive = e.Organisation.IsActive,
                OrganisationId = e.OrganisationId,
                Worksheets = e.Organisation.Projects.SelectMany(f => f.Worksheets).AsQueryable()
            })
            .ForPagedResult(page, pageSize);

        return Data(MapperService.ViewModelMapper.Map<PageResultSet<OrganisationWorksheetViewModel>>(usersInProject));
    }

    [HttpGet]
    [Route("GetProjectsOfOrg")]
    public IActionResult GetAssosiatedProjects(Guid organizationId, int page, int pageSize, string search = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var organisation = db.Organisations.Find(organizationId);
        if (organisation == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(organizationId).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        var projectsInOrgQuery = db.Projects
            .Where(e => e.IdOrganisation == organizationId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            projectsInOrgQuery = projectsInOrgQuery
                .Where(e => e.Name.Contains(search));
        }
        return Data(MapperService.ViewModelMapper.Map<PageResultSet<GetProjectModel>>(projectsInOrgQuery.OrderBy(e => e.Name).ForPagedResult(page, pageSize)));
    }

    [HttpPost]
    [Route("Delete")]
    public IActionResult Delete(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(id);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (org.IsDeleted)
        {
            return BadRequest("Organisation is deleted and cannot be changed");
        }

        if (!GetRolesInOrg(id).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        db.Organisations
        .Where(e => e.OrganisationId == id)
        .ExecuteUpdate(e => e.SetProperty(f => f.IsDeleted, true));
        return Data();
    }

    [HttpPost]
    [Route("Create")]
    public IActionResult Create([FromBody] OrganizationGroupViewModel model)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var nameExists = db.Organisations.Where(e => e.Name == model.OrganizationViewModel.Name)
            .FirstOrDefault();

        if (nameExists != null)
        {
            return BadRequest("There is already an Organisation with the same name");
        }

        if (model.OrganizationViewModel.IdParentOrganisation.HasValue)
        {
            var parentOrg = db.Organisations.Find(model.OrganizationViewModel.IdParentOrganisation.Value);

            if (parentOrg == null)
            {
                return BadRequest("This parent Organisation does not exist");
            }

            if (!GetRolesInOrg(model.OrganizationViewModel.IdParentOrganisation.Value).Contains(UserToOrgRoles.Administrator.Name))
            {
                return BadRequest("Org/NotPartOfOrg".AsTranslation());
            }
        }

        var address = MapperService.ViewModelMapper.Map<Address>(model.Address);
        model.Address = null;
        var org = MapperService.ViewModelMapper.Map<Organisation>(model.OrganizationViewModel);
        address.DateOfCreation = DateTime.UtcNow;
        using var transaction = db.Database.BeginTransaction();

        org.IdAddressNavigation = address;

        db.Add(org);

        var users = new List<OrganisationUserMap>(MapperService.ViewModelMapper.Map<OrganisationUserMap[]>(model.Users))
            {
                new()
                {
                    IdAppUser = User.GetUserId(),
                    IdOrganisation = org.OrganisationId,
                    IdRelation = UserToOrgRoles.Creator.Id
                },
                new()
                {
                    IdAppUser = User.GetUserId(),
                    IdOrganisation = org.OrganisationId,
                    IdRelation = UserToOrgRoles.Administrator.Id
                }
            };

        foreach (var organisationUserMap in users.DistinctBy(e => e.IdAppUser + "" + e.IdRelation))
        {
            organisationUserMap.IdOrganisation = org.OrganisationId;
            db.Add(organisationUserMap);
        }

        db.SaveChanges();
        transaction.Commit();

        return Data(MapperService.ViewModelMapper.Map<UpdateOrganizationViewModel>(org));
    }

    [HttpPost]
    [Route("Update")]
    public IActionResult Update([FromBody] OrganizationGroupViewModel organizationModel, Guid id)
    {
        if (organizationModel == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        using var db = EntitiesFactory.CreateDbContext();
        var org = db.Organisations.Find(id);

        if (org == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        if (org.IsDeleted)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!GetRolesInOrg(id).Contains(UserToOrgRoles.Administrator.Name))
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        if (organizationModel.OrganizationViewModel != null)
        {
            if (organizationModel.OrganizationViewModel.IdParentOrganisation.HasValue)
            {
                var parentOrg = db.Organisations.Find(organizationModel.OrganizationViewModel.IdParentOrganisation.Value);

                if (parentOrg == null)
                {
                    return BadRequest("This parent Organisation does not exist");
                }

                if (!GetRolesInOrg(organizationModel.OrganizationViewModel.IdParentOrganisation.Value)
                        .Contains(UserToOrgRoles.Administrator.Name))
                {
                    return BadRequest("Org/NotPartOfOrg".AsTranslation());
                }
            }
        }
        using var transaction = db.Database.BeginTransaction();
        if (organizationModel.OrganizationViewModel != null)
        {
            org = MapperService.ViewModelMapper.Map(organizationModel.OrganizationViewModel, org);
            db.Organisations.Where(e => e.OrganisationId == id)
                .ExecuteUpdate(f => f.SetProperty(e => e.SharedId, org.SharedId).SetProperty(e => e.IdParentOrganisation, org.IdParentOrganisation).SetProperty(e => e.IsActive, true));
        }

        if (organizationModel.Address != null)
        {
            var address = MapperService.ViewModelMapper.Map<Address>(organizationModel.Address);
            address.DateOfCreation = DateTime.UtcNow;
            address.IdOrganisation = org.OrganisationId;
            db.Add(address);
            org.IdAddressNavigation = address;
        }

        if (organizationModel.Users.Any())
        {
            foreach (var userMap in organizationModel.Users)
            {
                if (userMap.Type == EntityListState.Deleted)
                {
                    db.OrganisationUserMaps.Where(e => e.IdOrganisation == id && e.IdAppUser == userMap.Entity.IdAppUser && e.IdRelation == userMap.Entity.IdRelation)
                    .ExecuteDelete();
                }
                else if (userMap.Type == EntityListState.Added)
                {
                    db.Add(new OrganisationUserMap()
                    {
                        IdOrganisation = id,
                        IdAppUser = userMap.Entity.IdAppUser,
                        IdRelation = userMap.Entity.IdRelation
                    });
                }
            }
        }
        db.SaveChanges();
        transaction.Commit();
        return Data(MapperService.ViewModelMapper.Map<UpdateOrganizationViewModel>(org));
    }
}