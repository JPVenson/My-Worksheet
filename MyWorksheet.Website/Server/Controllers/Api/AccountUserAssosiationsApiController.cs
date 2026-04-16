using System;
using System.Linq;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Text;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Assosiation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Invites;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[RevokableAuthorize]
[Route("api/AccountUserAssociationApi")]
public class AccountUserAssociationApiControllerBase : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly ILocalFileProvider _fileProvider;
    private readonly IDbContextFactory<MyworksheetContext> _dbFactory;
    private readonly ITextService _textService;

    public AccountUserAssociationApiControllerBase(IMapperService mapper, ILocalFileProvider fileProvider, IDbContextFactory<MyworksheetContext> db,
        ITextService textService)
    {
        _mapper = mapper;
        _fileProvider = fileProvider;
        _dbFactory = db;
        _textService = textService;
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("GetAssociationRoles")]
    public IActionResult GetAssociationLookup()
    {
        return Data(_mapper.ViewModelMapper.Map<UserToUserRoleViewModel[]>(UserToUserRoles.YieldRoles().Skip(1)));
    }

    [Route("GetUserAssociationRoles")]
    [HttpGet]
    public async Task<IActionResult> GetUserAssociationRolesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return Data(db.UserAssosiatedRoleLookups);
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("GetInvites")]
    public async Task<IActionResult> GetInvitesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return Data(_mapper.ViewModelMapper.Map<AssosiationInviteModel[]>(db.AssosiationInvitations.Where(e => e.IdRequestingUser == User.GetUserId())));
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("PreviewInvite")]
    public async Task<IActionResult> PreviewInviteAsync(string inviteCode)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var tryRedeem = TryRedeem(inviteCode, User.GetUserId(), db);
        if (tryRedeem)
        {
            var invite = db.AssosiationInvitations.FirstOrDefault(f => f.ExternalId == inviteCode);
            var invitingUser = db.AppUsers.Find(invite.IdRequestingUser);
            var inviteType = UserToUserRoles.Find(invite.IdUserAssosiatedRoleLookup);

            var inviteRedeeModel = new AssosiationInviteRedeemingModel();
            inviteRedeeModel.ExternalId = invite.ExternalId;
            inviteRedeeModel.Account = _mapper.ViewModelMapper.Map<AccountApiUserPost>(invitingUser);
            inviteRedeeModel.Role = _mapper.ViewModelMapper.Map<UserToUserRoleViewModel>(inviteType);

            return Data(inviteRedeeModel);
        }

        return BadRequest(tryRedeem.Reason);
    }

    [NonAction]
    private static QuestionableBoolean TryRedeem(string inviteCode, Guid userId, MyworksheetContext db)
    {
        var invite = db.AssosiationInvitations.FirstOrDefault(f => f.ExternalId == inviteCode);
        if (invite == null)
        {
            return false.Because("AccountAssosiation/TryRedeem.Unkown".AsTranslation().ToString());
        }

        if (invite.Revoked)
        {
            return false.Because($"AccountAssosiation/TryRedeem.Expired".AsTranslation().ToString());
        }

        if (invite.ValidUntil.HasValue && invite.ValidUntil.Value < DateTime.UtcNow)
        {
            return false.Because("AccountAssosiation/TryRedeem.Expired2".AsTranslation().ToString());
        }

        if (invite.IdRequestingUser == userId)
        {
            return false.Because("AccountAssosiation/TryRedeem.Pun".AsTranslation().ToString());
        }

        var userAssosiations = db.UserAssoisiatedUserMaps
        .Where(f => f.IdParentUser == invite.IdRequestingUser && f.IdChildUser == userId && f.IdUserRelation == invite.IdUserAssosiatedRoleLookup)
            .FirstOrDefault();
        if (userAssosiations != null)
        {

            return false.Because("AccountAssosiation/TryRedeem.AssosiationExists".AsTranslation().ToString());
        }

        return true;
    }

    [RevokableAuthorize]
    [HttpPost]
    [Route("RevokeInvite")]
    public async Task<IActionResult> RevokeInviteAsync(Guid inviteId, string reason)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var hasInviteLink = db.AssosiationInvitations.Where(e => e.AssosiationInvitationId == inviteId)
            .FirstOrDefault();

        if (hasInviteLink == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (hasInviteLink.IdRequestingUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (hasInviteLink.Revoked)
        {
            return BadRequest("AccountAssosiation/TryRedeem.Expired".AsTranslation());
        }

        db.AssosiationInvitations.Where(e => e.AssosiationInvitationId == inviteId)
            .ExecuteUpdate(e => e.SetProperty(f => f.Revoked, true).SetProperty(f => f.RevokeReason, reason).SetProperty(f => f.RevokedDate, DateTime.Now));

        return Data();
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("CreateUserMappingInvite")]
    public async Task<IActionResult> CreateInviteLinkAsync(InviteCreationViewModel invite)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var hasInviteLink = await db.AssosiationInvitations
        .Where(e => e.IdRequestingUser == User.GetUserId() && e.Revoked == false)
        .CountAsync();


        if (hasInviteLink > 10)
        {
            return BadRequest("AccountAssosiation/Create.LimitReached".AsTranslation());
        }

        var inviteModel = new AssosiationInvitation()
        {
            ExternalId = Guid.NewGuid().ToString(),
            IdRequestingUser = User.GetUserId(),
            IdUserAssosiatedRoleLookup = invite.LinkType,
            ValidOnce = invite.ValidOnce,
            ValidUntil = invite.ValidUntil
        };
        db.AssosiationInvitations.Add(inviteModel);
        await db.SaveChangesAsync();
        return Data(_mapper.ViewModelMapper.Map<AssosiationInviteModel>(inviteModel));
    }

    [RevokableAuthorize]
    [HttpPost]
    [Route("RedeemInviteLink")]
    public async Task<IActionResult> RedeemInviteLinkAsync(string externalId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var hasInviteLink = db.AssosiationInvitations
            .Where(f => f.ExternalId == externalId)
            .FirstOrDefault();
        var tryRedeem = TryRedeem(externalId, User.GetUserId(), db);
        if (!tryRedeem)
        {
            return BadRequest(tryRedeem.Reason);
        }

        var entity = new UserAssoisiatedUserMap()
        {
            IdChildUser = User.GetUserId(),
            IdParentUser = hasInviteLink.IdRequestingUser,
            IdUserRelation = hasInviteLink.IdUserAssosiatedRoleLookup,
            IdInvite = hasInviteLink.AssosiationInvitationId
        };
        db.UserAssoisiatedUserMaps.Add(entity);

        if (hasInviteLink.ValidOnce)
        {
            hasInviteLink.Revoked = true;
            hasInviteLink.RevokeReason = _textService.Compile("AccountAssosiation/Redeem.Code")
                .ToString();
            hasInviteLink.RevokedDate = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return Data();
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("GetAssociation")]
    public async Task<IActionResult> GetAssociationAsync(int page, int take, string search = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var hasConstalationQuery = db.UserAssoisiatedUserMaps
            .Include(e => e.IdParentUserNavigation)
            .Include(e => e.IdChildUserNavigation)
            .Include(e => e.IdUserRelationNavigation)
            .Where(e => e.IdParentUser == User.GetUserId() || e.IdChildUser == User.GetUserId());

        if (!string.IsNullOrWhiteSpace(search))
        {
            //hasConstalationQuery = hasConstalationQuery.And.Column(f =>f.)
        }

        var forPagedResult = hasConstalationQuery.OrderBy(e => e.IdParentUser)
            .ForPagedResult(page, take);
        return Data(_mapper.ViewModelMapper.Map<PageResultSet<UserToUserAssosiationViewModel>>(forPagedResult));
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("IsAdministrated")]
    public IActionResult IsAdministratedBy()
    {
        using var db = _dbFactory.CreateDbContext();
        var hasConstalation = db.UserAssoisiatedUserMaps.Where(f => f.IdChildUser == User.GetUserId())
            .Where(f => f.IdUserRelation == UserToUserRoles.Administrated.Id)
            .FirstOrDefault();
        if (hasConstalation == null)
        {
            return Data();
        }

        return Data(_mapper.ViewModelMapper.Map<AccountApiGet>(db.AppUsers.Find(hasConstalation.IdParentUser)));
    }

    [RevokableAuthorize]
    [HttpPost]
    [Route("RemoveAdministration")]
    public IActionResult RemoveAdministration()
    {
        using var db = _dbFactory.CreateDbContext();
        var hasConstalation = db.UserAssoisiatedUserMaps.Where(f => f.IdChildUser == User.GetUserId())
            .Where(f => f.IdUserRelation == UserToUserRoles.Administrated.Id)
            .FirstOrDefault();
        if (hasConstalation == null)
        {
            return BadRequest("AccountAssosiation/RemoveAdminstrator.NoActive".AsTranslation());
        }

        db.UserAssoisiatedUserMaps.Where(e => e.UserAssoisiatedUserMapId == hasConstalation.UserAssoisiatedUserMapId).ExecuteDelete();

        return Data();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/AddUserAssociation")]
    [HttpPost]
    public IActionResult AddAssociation(Guid parentId, Guid childId, Guid roleId)
    {
        using var db = _dbFactory.CreateDbContext();
        var hasConstalation = db.UserAssoisiatedUserMaps.Where(f => f.IdParentUser == parentId).Where(f => f.IdChildUser == childId).Where(f => f.IdUserRelation == roleId)
            .FirstOrDefault();
        if (hasConstalation != null)
        {
            return BadRequest("This Assosiation does already exist");
        }

        var userAssoisiatedUserMap = new UserAssoisiatedUserMap()
        {
            IdParentUser = parentId,
            IdChildUser = childId,
            IdUserRelation = roleId
        };
        db.Add(userAssoisiatedUserMap);
        db.SaveChanges();

        return Data(userAssoisiatedUserMap);
    }

    [RevokableAuthorize()]
    [Route("RemoveUserAssociation")]
    [HttpPost]
    public IActionResult RemoveAssociation(Guid id)
    {
        using var db = _dbFactory.CreateDbContext();
        var userId = User.GetUserId();
        var assosiation = db.UserAssoisiatedUserMaps.Find(id);
        if (assosiation == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (assosiation.IdParentUser != userId)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return RemoveAssosiation(userId, assosiation.IdChildUser, assosiation.IdUserRelation);
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/RemoveUserAssociation")]
    [HttpPost]
    public IActionResult RemoveAssosiation(Guid parentId, Guid childId, Guid roleId)
    {
        using var db = _dbFactory.CreateDbContext();
        var hasConstalation = db.UserAssoisiatedUserMaps.Where(f => f.IdParentUser == parentId).Where(f => f.IdChildUser == childId).Where(f => f.IdUserRelation == roleId)
            .FirstOrDefault();
        if (hasConstalation == null)
        {
            return BadRequest("This Assosiation does not exist");
        }

        if (roleId == UserToUserRoles.Self.Id)
        {
            return BadRequest("You cannot delete yourself");
        }

        db.UserAssoisiatedUserMaps.Where(e => e.UserAssoisiatedUserMapId == hasConstalation.UserAssoisiatedUserMapId).ExecuteDelete();

        return Data();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/GetUserAssociation")]
    [HttpGet]
    public IActionResult GetUserAssociation(Guid userId)
    {
        using var db = _dbFactory.CreateDbContext();
        var appUsers = db.UserAssoisiatedUserMaps.Where(f => f.IdParentUser == userId);
        return Data(_mapper.ViewModelMapper.Map<UserAssosiationModel[]>(appUsers));
    }

    [Route("GetUserAssociation")]
    [HttpGet]
    public IActionResult GetUserAssociation()
    {
        return GetUserAssociation(User.GetUserId());
    }
}