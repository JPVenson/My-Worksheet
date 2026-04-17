using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Helper;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Google.ReCapture2;
using MyWorksheet.Website.Server.Services.Mail;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;
using MyWorksheet.Website.Server.Services.MailDomainChecker;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/RegistrationApi")]
public class RegistrationApiController : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMailServiceProvider _mailService;
    private readonly IMapperService _mapper;
    private readonly IOptions<AppServerSettings> _serverSettings;
    private readonly IOptions<IsItSettings> _isItSettings;
    private readonly IUserQuotaService _userQuotaService;
    private readonly ILogger<RegistrationApiController> _appLogger;
    private readonly IActivityService _activityService;
    private readonly IBlacklistMailDomainService _blacklistMailDomainService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ActivatorService _activatorService;
    private IBlobManagerService _blobManagerService;
    private INumberRangeService _numberRangeService;

    public RegistrationApiController(IMapperService mapper,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IMailServiceProvider mailService,
        IOptions<AppServerSettings> serverSettings,
        IOptions<IsItSettings> isItSettings,
        IUserQuotaService userQuotaService,
        ILogger<RegistrationApiController> appLogger,
        IActivityService activityService,
        IBlacklistMailDomainService blacklistMailDomainService,
        UserManager<AppUser> userManager,
        ActivatorService activatorService,
        IBlobManagerService blobManagerService,
        INumberRangeService numberRangeService)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
        _mailService = mailService;
        _serverSettings = serverSettings;
        _isItSettings = isItSettings;
        _userQuotaService = userQuotaService;
        _appLogger = appLogger;
        _activityService = activityService;
        _blacklistMailDomainService = blacklistMailDomainService;
        _userManager = userManager;
        _activatorService = activatorService;
        _blobManagerService = blobManagerService;
        _numberRangeService = numberRangeService;
    }

    [Route("TestUser")]
    [HttpPost]
    public async Task<IActionResult> CreateTestUser(TestUserAccountCreate model)
    {
        if (!_serverSettings.Value.FeatureSwitch.TestRegistration.Enabled)
        {
            return BadRequest("Register/Disabled".AsTranslation());
        }

        var questiableBoolean =
            GoogleReCapcha.Validate(model.Recapture, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        if (questiableBoolean == false)
        {
            return BadRequest(questiableBoolean.Reason);
        }

        using var db = _dbContextFactory.CreateDbContext();
        var mailCheck = await Algorithms.CheckMail(model.MailAddress, db, _blacklistMailDomainService);
        if (!mailCheck)
        {
            return BadRequest(mailCheck.Reason);
        }

        var username = Algorithms.GenerateUsername(model.MailAddress, db);
        var generateTempPassword = Algorithms.GeneratePassword(_userManager);

        if (!await _mailService.ApplicationMailService.SendMail(_activatorService.ActivateType<OutgoingTestAccountRegistrationMail>
                (generateTempPassword, model.MailAddress, username, null), User.GetUserId(), model.MailAddress))
        {
            return BadRequest("Register/MailFailed".AsTranslation());
        }
        AppUser appUser;

        try
        {
            appUser = AccountHelper.CreateUser(db, new AccountApiUserCreate
            {
                RegionId = _serverSettings.Value.User.Create.DefaultRegion,
                Username = username,
                UserPlainTextPassword = generateTempPassword,
                NeedPasswordReset = false
            }, AccountHelper.CreateDefaultAddress(), _serverSettings.Value.User.Create.DefaultRoles, _appLogger, _userQuotaService, _numberRangeService, true);
            appUser.Email = model.MailAddress;
            db.Update(appUser);
            db.Add(new UserRoleMap
            {
                IdRole = Roles.OnDemandUser.Id,
                IdUser = appUser.AppUserId
            });
            await db.SaveChangesAsync().ConfigureAwait(false);
            await _userQuotaService.Subtract(appUser.AppUserId, 1, Quotas.Project);
            await _userQuotaService.Subtract(appUser.AppUserId, 1, Quotas.Worksheet);
        }
        catch (Exception e)
        {
            _appLogger.LogCritical("Error due the Registration.", LoggerCategories.Registration.ToString(),
                new Dictionary<string, string>
                {
                    {
                        "Exception:", e.ToString()
                    },
                    {
                        "Userdata:", JsonConvert.SerializeObject(model)
                    }
                });

            return BadRequest("Register/UnexpectedError".AsTranslation());
        }

        await _activityService.CreateActivity(ActivityTypes.TestAccountCreated.CreateActivity(db, appUser));

        return Data("Register.Done".AsTranslation());
    }

    [Route("Register")]
    [HttpPost]
    public async Task<IActionResult> Register(RegistrationPostModel model)
    {
        if (!_serverSettings.Value.FeatureSwitch.Registration.Enabled)
        {
            return BadRequest("The Registration is Currently disabled");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var questiableBoolean =
            GoogleReCapcha.Validate(model.Recapture, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        if (questiableBoolean == false)
        {
            return BadRequest(questiableBoolean.Reason);
        }

        using var db = _dbContextFactory.CreateDbContext();
        var mailCheck = await Algorithms.CheckMail(model.Email, db, _blacklistMailDomainService);
        if (!mailCheck)
        {
            return BadRequest(mailCheck.Reason);
        }

        var country = db.PromisedFeatureRegions.Find(model.RegionId);

        if (country == null)
        {
            return BadRequest("The selected Country does not exist. Please check you input");
        }

        //ok register now!
        var decodedPassword = ChallangeUtil.StringToByteArrayFastest(model.Password);
        var clearTextPassword = Encoding.UTF8.GetString(decodedPassword);

        var hits = new List<string>();
        var pwCheck = PasswordCheck.FullPasswordCheck(clearTextPassword, hits);

        if (!pwCheck)
        {
            return BadRequest(hits.Aggregate((e, f) => e + Environment.NewLine + f));
        }

        if (db.AppUsers.Where(f => f.Username == model.Username).Any())
        {
            return BadRequest("The username is Already in use. Choose another");
        }

        AppUser appUser = null;
        try
        {
            var transaction = await db.Database.BeginTransactionAsync();
            await using (transaction.ConfigureAwait(false))
            {
                appUser = AccountHelper.CreateUser(db, new AccountApiUserCreate
                {
                    RegionId = model.RegionId,
                    Username = model.Username,
                    UserPlainTextPassword = clearTextPassword,
                    NeedPasswordReset = false
                }, _mapper.ViewModelMapper.Map<Address>(model.Address), _serverSettings.Value.User.Create.DefaultRoles, _appLogger, _userQuotaService, _numberRangeService);
                appUser.Email = model.Email;
                db.Update(appUser);

                var mail = _activatorService.ActivateType<OutgoingRegistrationMail>(clearTextPassword,
                    model.Email,
                    model.Username,
                    model.Address);

                if (await _mailService.ApplicationMailService.SendMail(mail, User.GetUserId(), model.Email))
                {
                    if (_isItSettings.Value.Free)
                    {
                        await _userQuotaService.Subtract(appUser.AppUserId, 1, Quotas.Project);
                        await _userQuotaService.Subtract(appUser.AppUserId, 1, Quotas.Worksheet);
                        await _activityService.CreateActivity(ActivityTypes.AccountCreatedGiftGranted.CreateActivity(db, appUser));
                    }

                    return Data("Successfuly registered. Please check your mails for the Confirmation mail");
                }
                await transaction.RollbackAsync().ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            _appLogger.LogCritical("Error due the Registration.", LoggerCategories.Registration.ToString(),
                new Dictionary<string, string>
                {
                    {
                        "Exception:", e.ToString()
                    },
                    {
                        "Userdata:", JsonConvert.SerializeObject(model)
                    }
                });

            if (appUser != null)
            {
                AccountHelper.DeleteUser(appUser, db, _blobManagerService);
            }

            return BadRequest(
                "Oh no. Something absolutly unexpected happend. We have created a log about this event and will investigate it!");
        }

        return BadRequest("Oh we had trouble to send your Confirmation mail. Please check you mail address");
    }
}

//var hashPw = Algorithms.HashPassword(generateTempPassword, model.Username);
//var user = new AppUser();
//user.Username = model.Username;
//user.Email = casedMail;
//user.PasswordHash = hashPw;
//user.IsAktive = true;
//user.NeedPasswordReset = true;
//var insertWithSelect = _db.InsertWithSelect(user);
//_db.Insert(new UserRoleMap() { IdRole = Roles.Kunde.Id, IdUser = insertWithSelect.AppUserID });