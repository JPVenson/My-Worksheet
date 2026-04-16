using System;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[RevokableAuthorize]
[Route("api/AddressApi")]
public class AddressApiControllerBase : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public AddressApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet]
    [Route("MyAddress")]
    public IActionResult GetByCurrent()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var user = db.AppUsers
            .Include(e => e.IdAddressNavigation)
            .FirstOrDefault(e => e.AppUserId == User.GetUserId());
        if (user.IdAddressNavigation is null)
        {
            return Data(new AddressModel());
        }
        return Data(_mapper.ViewModelMapper.Map<AddressModel>(user.IdAddressNavigation));
    }

    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/UserAddress")]
    public IActionResult GetByUserId(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<AddressModel[]>(db.AppUsers
            .Include(e => e.Addresses)
            .FirstOrDefault(e => e.AppUserId == id).Addresses));
    }

    [HttpGet]
    [RevokableAuthorize()]
    [Route("UserAddress")]
    public IActionResult GetByUserRefId(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var isLinkedUser = db.UserAssoisiatedUserMaps.Where(f => f.IdParentUser == User.GetUserId())
            .Where(f => f.IdChildUser == id).Any();

        if (isLinkedUser)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var user = db.AppUsers
            .Include(e => e.IdAddressNavigation)
            .FirstOrDefault(e => e.AppUserId == User.GetUserId());
        return Data(_mapper.ViewModelMapper.Map<AddressModel>(user.IdAddressNavigation));
    }

    [HttpGet]
    [RevokableAuthorize()]
    [Route("OrganizationAddress")]
    public IActionResult Get(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var organisation = db.Organisations.Find(id);

        if (organisation == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var partOfOrg = db.OrganisationUserMaps
            .Where(f => f.IdAppUser == User.GetUserId())
            .Where(e => e.IdRelation == UserToOrgRoles.Administrator.Id || e.IdRelation == UserToOrgRoles.Creator.Id)
            .FirstOrDefault();

        if (partOfOrg == null)
        {
            return BadRequest("Org/NotPartOfOrg".AsTranslation());
        }

        return Data(_mapper.ViewModelMapper.Map<AddressModel>(db.Addresses.Find(organisation.IdAddress)));
    }

    [HttpPost]
    [Route("Update")]
    public IActionResult PutAddress([FromBody] UpdateAddressModel userModel)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var user = db.AppUsers
            .Include(e => e.IdAddressNavigation)
            .FirstOrDefault(e => e.AppUserId == User.GetUserId());

        var address = db.Addresses.Find(user.IdAddress);
        if (address.IdAppUser != user.AppUserId)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        userModel.AddressId = user.IdAddress.Value;
        _mapper.ViewModelMapper.Map(userModel, address);
        using var transaction = db.Database.BeginTransaction();

        address.DateOfCreation = DateTime.UtcNow;
        user.IdAddressNavigation = address;
        db.Add(address);

        db.SaveChanges();
        transaction.Commit();

        return Data(_mapper.ViewModelMapper.Map<AddressModel>(address));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/Update")]
    [HttpPost]
    public IActionResult PutAddress([FromBody] AddressModel userModel, Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var address = db.Addresses.Find(userModel.AddressId);
        var user = db.AppUsers.Find(userId);
        _mapper.ViewModelMapper.Map(userModel, address);
        using var transaction = db.Database.BeginTransaction();

        address.DateOfCreation = DateTime.UtcNow;
        user.IdAddressNavigation = address;
        db.Add(address);

        db.SaveChanges();
        transaction.Commit();

        return Data(_mapper.ViewModelMapper.Map<AddressModel>(address));
    }
}