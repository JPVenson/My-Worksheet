using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/RoleApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName)]
public class RoleWebApiControllerBase : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public RoleWebApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("GetRoles")]
    public IActionResult GetRolesOfUser(Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userMaps = db.UserRoleMaps.Where(f => f.IdUser == userId).Select(f => f.IdRole).ToArray();
        if (!userMaps.Any())
        {
            return Data(new List<Role>());
        }

        var roles = db.Roles.Where(e => userMaps.Contains(e.RoleId)).ToArray();
        return Data(roles);
    }

    [Route("GetRoles")]
    public IActionResult GetRoles()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var roles = db.Roles;
        return Data(roles.ToArray());
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("AddRole")]
    [HttpPost]
    public IActionResult AddRole(Guid userId, Guid role)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var user = db.AppUsers.Where(e => e.AppUserId == userId)
            .FirstOrDefault();
        if (user == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        var hasRole =
            db.UserRoleMaps.Where(e => e.IdRole == role && e.IdUser == userId).Any();

        if (hasRole)
        {
            return BadRequest("Role/AddRole.UserHasRole".AsTranslation());
        }

        db.Add(new UserRoleMap()
        {
            IdUser = user.AppUserId,
            IdRole = role
        });
        db.SaveChanges();

        return Data();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("RemoveRole")]
    [HttpPost]
    public IActionResult RemoveRole(Guid userId, Guid role)
    {
        if (userId == Roles.Administrator.Id && role == Roles.Administrator.Id)
        {
            return BadRequest("Role/RemoveRole.SuperAdmin".AsTranslation());
        }

        using var db = _dbContextFactory.CreateDbContext();
        db.UserRoleMaps
            .Where(f => f.IdRole == role)
            .Where(f => f.IdUser == userId)
            .ExecuteDelete();
        return Data();
    }
}