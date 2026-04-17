using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Shared.Services.PriorityQueue;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.Auth;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.ExecuteLater.Actions;
using MyWorksheet.Website.Server.Services.Google.ReCapture2;
using MyWorksheet.Website.Server.Services.Mail;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Auth;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UAParser;
using Roles = MyWorksheet.Webpage.Helper.Roles.Roles;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/AccountApi")]
public class AccountControllerBase : RestApiControllerBase<AppUser, AccountApiAdminGet>
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbFactory;
    private readonly ILogger<AccountControllerBase> _logger;
    private readonly IServerPriorityQueueManager _actionDispatcher;
    private readonly IOptions<TransformationSettings> _transformationSettings;
    private readonly IOptions<GoogleSettings> _googleSettings;
    private readonly IOptions<AppServerSettings> _serverSettings;
    private readonly TokenManager _tokenManager;
    private readonly IResetPasswordManager _resetPasswordManager;
    private readonly IActivityService _activityService;
    private readonly IMailServiceProvider _mailServiceProvider;
    private IUserQuotaService _userQuotaService;
    private IBlobManagerService _userBlobService;
    private readonly ActivatorService _activatorService;
    private INumberRangeService _numberRangeService;

    public AccountControllerBase(IMapperService mapper,
        IDbContextFactory<MyworksheetContext> dbFactory,
        ILogger<AccountControllerBase> logger,
        IServerPriorityQueueManager actionDispatcher,
        IOptions<TransformationSettings> transformationSettings,
        IOptions<GoogleSettings> googleSettings,
        IOptions<AppServerSettings> serverSettings,
        TokenManager tokenManager,
        IResetPasswordManager resetPasswordManager,
        IActivityService activityService,
        IMailServiceProvider mailServiceProvider,
        IUserQuotaService userQuotaService,
        IBlobManagerService userBlobService,
        ActivatorService activatorService,
        INumberRangeService numberRangeService)
        : base(dbFactory, mapper)
    {
        _mapper = mapper;
        _dbFactory = dbFactory;
        _logger = logger;
        _actionDispatcher = actionDispatcher;
        _transformationSettings = transformationSettings;
        _googleSettings = googleSettings;
        _serverSettings = serverSettings;
        _tokenManager = tokenManager;
        _resetPasswordManager = resetPasswordManager;
        _activityService = activityService;
        _mailServiceProvider = mailServiceProvider;
        _userQuotaService = userQuotaService;
        _userBlobService = userBlobService;
        _activatorService = activatorService;
        _numberRangeService = numberRangeService;
        SearchWithUserReference = false;
    }

    [HttpPost]
    [Route("ResetPasswordRequest")]
    public async Task<IActionResult> PasswordRequest([FromBody] AccountPasswordRequestPost data)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var responseText = "Account/PasswordReset.DefaultText".AsTranslation();
        var recapture = GoogleReCapcha.Validate(data.Recapture,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        if (recapture == false)
        {
            _logger.LogError("The google Vertification has Failed for this Request. Password Request",
                LoggerCategories.Google.ToString(), new Dictionary<string, string>()
                {
                    {"Reason", recapture.Reason},
                    {"Ip", recapture.Reason},
                    {"Data", JsonConvert.SerializeObject(data)}
                });

            return BadRequest("Common/GoogleVertficationFailed".AsTranslation());
        }

        var findUser = db.AppUsers.Where(f => f.Username == data.Username).FirstOrDefault();
        if (findUser == null)
        {
            return Data(responseText);
        }
        if (!string.Equals(findUser.Email, data.Email, StringComparison.CurrentCultureIgnoreCase))
        {
            return Data(responseText);
        }

        var agentString = HttpContext.Request.Headers["User-Agent"];
        if (string.IsNullOrWhiteSpace(agentString))
        {
            return NotFound();
        }

        var browserCapabilities = Parser.GetDefault().Parse(agentString);

        await _actionDispatcher.Enqueue(PriorityManagerLevel.Later, SendPasswordRequest.ActionKey, findUser.AppUserId, new Dictionary<string, object>()
        {
            {"name", db.Addresses.Find(findUser.IdAddress).FirstName },
            {"mailAddress", db.Addresses.Find(findUser.IdAddress).EmailAddress },
            {"browser_name", browserCapabilities?.Device?.ToString() ?? "Unknown" },
            {"date", DateTime.UtcNow.ToString("R") },
            {"operating_system", browserCapabilities?.OS?.ToString() ?? "Unknown" },
        });

        return Data(responseText);
    }

    /// <summary>
    /// Gets the Page settings that should be applyed globaly to any client that is using the WebApi
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("PageSettings")]
    public IActionResult GetPageSettings()
    {
        return Data(new ClientSettingsModel()
        {
            Realm = _transformationSettings.Value.Realm,
            VersionType = _transformationSettings.Value.Version,
            Version = typeof(MyWorksheet.Website.Server.Shared.AppStartup.ConfigurateSettingsService).Assembly.GetName().Version,
            RecaptchaKey = _googleSettings.Value.Recaptcha.Keys.Public,
            //MaintainceModeUpdate = _mapper.ViewModelMapper.Map<MaintainceModeViewModel>(WorksheetStartup.MaintainceMode)
        });
    }

    protected override AppUser GetByUser(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return db.AppUsers.Where(e => e.AppUserId == id)
            .FirstOrDefault();
    }

    protected override AppUser[] GetAllByUser()
    {
        using var db = EntitiesFactory.CreateDbContext();
        return db.AppUsers
            .ToArray();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/Create")]
    [HttpPost]
    public IActionResult CreateUser([FromBody] AccountApiUserCreate model)
    {
        using var db = EntitiesFactory.CreateDbContext();
        try
        {
            var user = AccountHelper.CreateUser(db, model, AccountHelper.CreateDefaultAddress(),
                _serverSettings.Value.User.Create.DefaultRoles,
                _logger,
                _userQuotaService,
                _numberRangeService);
            return Data(_mapper.ViewModelMapper.Map<AccountApiAdminGet>(user));
        }
        catch (Exception e)
        {
            return base.BadRequest(e);
        }
    }

    /// <summary>
    /// Allowes You to change your password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPost]
    [Route("ChangePassword")]
    [RevokableAuthorize]
    public IActionResult ChangePassword([FromBody] AccountApiUserChangePassword model)
    {
        using var db = EntitiesFactory.CreateDbContext();
        if (model.ConfirmPassword != model.NewPassword)
        {
            return BadRequest("Account/ChangePassword.Mismatch".AsTranslation());
        }

        var user =
            db.AppUsers.Where(e => e.Username == User.Identity.Name)
                .FirstOrDefault();
        if (user == null)
        {
            return BadRequest();
        }
        if (user.IsTestUser)
        {
            return BadRequest("Account/ChangePassword.YouAreTestUser".AsTranslation());
        }

        var oldPw = ChallangeUtil.HashPassword(model.OldPassword, user.Username);

        if (!oldPw.SequenceEqual(user.PasswordHash))
        {
            return BadRequest("Account/ChangePassword.OldPasswordMismatch".AsTranslation());
        }

        user.NeedPasswordReset = false;
        user.PasswordHash = ChallangeUtil.HashPassword(model.NewPassword, user.Username);
        db.Update(user);

        _tokenManager.LogoutUser(User.GetUserId());

        return Data();
    }

    /// <summary>
    /// Allowes You to change your password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPost]
    [Route("ResetPassword")]
    public IActionResult ResetPassword([FromBody] AccountApiUserResetPassword model)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var questionableBoolean = GoogleReCapcha.Validate(model.ResetToken,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());

        if (!questionableBoolean)
        {
            return BadRequest("Common/GoogleVertficationFailed".AsTranslation());
        }

        if (model.ConfirmPassword != model.NewPassword)
        {
            return BadRequest("Account/ChangePassword.Mismatch".AsTranslation());
        }

        if (!_resetPasswordManager.RedeemPasswordToken(model.ResetToken, out var userId))
        {
            return BadRequest("Account/ResetPassword.InvalidToken".AsTranslation());
        }

        var user = db.AppUsers.Find(userId);
        if (user == null)
        {
            return BadRequest();
        }
        if (user.IsTestUser)
        {
            return BadRequest("Account/ChangePassword.YouAreTestUser".AsTranslation());
        }
        if (!user.IsAktive)
        {
            return BadRequest("Account/ResetPassword.InactiveAccount".AsTranslation());
        }

        user.NeedPasswordReset = false;
        user.PasswordHash = ChallangeUtil.HashPassword(model.NewPassword, user.Username);
        db.Update(user);

        _tokenManager.LogoutUser(User.GetUserId());

        return Data();
    }

    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/ChangePassword")]
    public IActionResult ChangePasswordAdmin([FromBody] AccountApiUserChangeUserPassword model)
    {
        using var db = EntitiesFactory.CreateDbContext();
        if (model.ConfirmPassword != model.NewPassword)
        {
            return BadRequest("Account/ChangePassword.Mismatch".AsTranslation());
        }


        var user =
            db.AppUsers.Where(e => e.Username == User.Identity.Name)
                .FirstOrDefault();
        if (user == null)
        {
            return BadRequest();
        }

        user.PasswordHash = ChallangeUtil.HashPassword(model.NewPassword, user.Username);
        db.Update(user);

        return Data();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/Delete")]
    [HttpPost]
    public void Delete(string id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var user = db.AppUsers.Where(f => f.Username == id).FirstOrDefault();
        user.IsAktive = false;
        db.Update(user);
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/DeleteAccount")]
    [HttpPost]
    public IActionResult DeleteAccount(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        AccountHelper.DeleteUser(db.AppUsers.Find(id), db,
            _userBlobService);
        return Data();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/UpdateAccount")]
    public IActionResult UpdateAccount(AccountApiAdminGet userModel)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var user = db.AppUsers.Find(userModel.UserID);
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _mapper.ViewModelMapper.Map(userModel, user);
        db.Update(user);
        return Ok();
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/GetAll")]
    [HttpGet]
    public IActionResult GetAllAdmin(int page, int take, bool includeTestUsers = false, string search = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var appUsers = db.AppUsers.Where(f => f.IsTestUser == includeTestUsers);
        if (!string.IsNullOrWhiteSpace(search))
        {
            return Data(_mapper.ViewModelMapper.Map<PageResultSet<AccountApiAdminGet>>(appUsers.Where(f => f.Username.Contains(search)).OrderBy(f => f.Username).ForPagedResult(page, take)));
        }
        else
        {
            return Data(_mapper.ViewModelMapper.Map<PageResultSet<AccountApiAdminGet>>(appUsers.OrderBy(f => f.Username).ForPagedResult(page, take)));
        }
    }

    [Route("GetAll")]
    [HttpGet]
    [RevokableAuthorize]
    public IActionResult GetAllUser(int page, int take, string search = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var assosiatedUsers = db.UserAssoisiatedUserMaps
            .Where(f => f.IdParentUser == User.GetUserId())
            .Select(f => f.IdChildUser)
            .ToArray();

        var users = db.AppUsers.Where(e => assosiatedUsers.Contains(e.AppUserId) && e.IsAktive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            users = users.Where(f => f.Username.Contains(search));
        }

        var result = users.OrderBy(f => f.Username).ForPagedResult(page, take);

        return Data(_mapper.ViewModelMapper.Map<PageResultSet<AccountApiAdminGet>>(result));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/GetUserActions")]
    public IActionResult GetUserActions(Guid userId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return
            Data(_mapper.ViewModelMapper.Map<UserActionAdminGet[]>(
                db.UserActions.Where(e => e.IdUser == userId)
                    .ToArray()));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/GetCounterInfos")]
    [HttpGet]
    public IActionResult GetCounterInfos(Guid userId, int? quotaType = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var quota = db.UserQuota.Where(f => f.IdAppUser == userId);

        if (quotaType.HasValue)
        {
            quota = quota.Where(f => f.QuotaType == quotaType.Value);
        }

        return Data(_mapper.ViewModelMapper.Map<UserQuotaViewModel[]>(quota.ToArray()));
    }

    [RevokableAuthorize()]
    [Route("GetCounterInfos")]
    [HttpGet]
    public IActionResult GetCounterInfos(int? quotaType = null)
    {
        return GetCounterInfos(User.GetUserId(), quotaType);
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/User")]
    public IActionResult Get(Guid userId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<AccountApiAdminGet>(db.AppUsers.Find(userId)));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpPost]
    [Route("Admin/Update")]
    public void UpdateUser([FromBody] AccountApiAdminGet postData)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var entity = db.AppUsers.Find(postData.UserID);
        _mapper.ViewModelMapper.Map(postData, entity);
        db.Update(entity);
    }

    [HttpGet]
    [RevokableAuthorize]
    [Route("CurrentUserData")]
    public IActionResult GetCurrentUserData()
    {
        using var db = EntitiesFactory.CreateDbContext();
        var user =
            db.AppUsers.Where(e => e.Username == User.Identity.Name)
                .FirstOrDefault();

        var setting = new PageSetting();
        setting.UidToken = Guid.NewGuid().ToString("D");
        _logger.LogInformation("Created Uid Token", LoggerCategories.Server.ToString(), new Dictionary<string, string>()
        {
            {"User", User?.Identity?.Name},
            {"Token", setting.UidToken}
        });
        if (User != null && user != null)
        {
            setting.IsLoggedin = User.Identity.IsAuthenticated;
            setting.IsAdmin = User.IsInRole(Roles.AdminRoleName);

            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                if (identity.HasClaim(f => f.Type == Claims.UserPersistClaimId && f.Value == "True"))
                {
                    setting.Persist = false;
                }
                setting.SessionTimeout = identity.FindAll(ClaimTypes.Expiration).LastOrDefault()?.Value;
                setting.IssuedDate = identity.FindAll(Claims.Issued).LastOrDefault()?.Value;
                setting.ServerSessionTimeout = identity.FindAll(Claims.ServerRefreshExpiresClaimId).LastOrDefault()?.Value;
                setting.Roles = identity.FindAll(ClaimTypes.Role).Select(e => e.Value).ToList();
            }
        }

        var userApiModel = _mapper.ViewModelMapper.Map<AccountApiUserGetInfo>(user);
        return Data(new AccountApiUserGet()
        {
            PageSettings = setting,
            UserInfo = userApiModel
        });
    }

    [RevokableAuthorize()]
    [HttpPost]
    [Route("UpdateCurrentUserData")]
    public async Task<IActionResult> PostCurrentUserData([FromBody] AccountApiUserPost postData)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var entity = db.AppUsers.Where(e => e.AppUserId == User.GetUserId()).FirstOrDefault();
        if (!entity.Email.Equals(postData.Email))
        {
            var mailService = _mailServiceProvider.ApplicationMailService;
            var sendMail = mailService.SendMail(_activatorService.ActivateType<NotifyMailChangedMail>(postData.Email), User.GetUserId(), entity.Email);

            var targetUrl = _serverSettings.Value.Realm.PrimaryRealm
                            + "/api/AccountApi/ConfirmEmailAddress?code="
                            + Uri.EscapeUriString(GenerateMailConfirmCode(postData.Email, entity.Email, entity.PasswordHash, entity.AppUserId))
                            + "&"
                            + "mail="
                            + Uri.EscapeUriString(postData.Email)
                            + "&"
                            + "changeId="
                            + entity.AppUserId;

            var result = await mailService.SendMail(_activatorService.ActivateType<ConfirmMailMail>(targetUrl), User.GetUserId(), postData.Email);
            await sendMail;
            if (!result)
            {
                return BadRequest(result.Reason);
            }
            postData.Email = entity.Email;
        }
        _mapper.ViewModelMapper.Map(postData, entity);
        db.Update(entity);
        return Data();
    }

    [HttpGet]
    [Route("ConfirmEmailAddress")]
    public IActionResult ConfirmMailAddress(string code, string mail, Guid changeId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var user = db.AppUsers.Where(f => f.AppUserId == changeId).FirstOrDefault();
        if (user == null)
        {
            return Redirect("/home?invalidMail=true");
        }

        var genCode = GenerateMailConfirmCode(mail, user.Email, user.PasswordHash, user.AppUserId);
        if (!genCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))
        {
            return Redirect("/home?mailConfirmed=false");
        }

        db.AppUsers.Where(e => e.AppUserId == user.AppUserId)
            .ExecuteUpdate(f =>
                f.SetProperty(e => e.Email, mail)
                .SetProperty(e => e.MailVerified, true)
                .SetProperty(e => e.MailVerifiedAt, DateTime.UtcNow)
                .SetProperty(e => e.MailVerifiedCounter, 0)
            );
        return Redirect("/home?mailConfirmed=true");
    }

    private string GenerateMailConfirmCode(string mail, string oldmail, byte[] hashedPassword, Guid appUserId)
    {
        return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(mail).Concat(Encoding.UTF8.GetBytes(oldmail)).Concat(SHA384.Create().ComputeHash(hashedPassword))
                .Concat(Encoding.UTF8.GetBytes(appUserId.ToString())).ToArray())
            .Select(e => e.ToString("X2").ToUpper()).Aggregate((e, f) => e + f);
    }
}