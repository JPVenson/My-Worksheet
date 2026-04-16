using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Shared.Auth;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Services.Jwt;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWorksheet.Website.Server.Settings;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/AuthorizeApi")]
public class AuthorizeApiController : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IAppLogger _logger;
    private IBlobManagerService _blobManagerService;
    private readonly TokenManager _tokenManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly UserManager<AppUser> _userManager;
    private readonly IOptions<TokenSettings> _tokenSettings;

    public AuthorizeApiController(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IAppLogger logger,
        TokenManager tokenManager,
        SignInManager<AppUser> signInManager,
        IBlobManagerService blobManagerService,
        IUserStore<AppUser> userStore,
        UserManager<AppUser> userManager, IOptions<TokenSettings> tokenSettings)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _tokenManager = tokenManager;
        _signInManager = signInManager;
        _blobManagerService = blobManagerService;
        _userStore = userStore;
        _userManager = userManager;
        _tokenSettings = tokenSettings;
    }

    [HttpPost]
    [Route("Challange")]
    public IActionResult GetChallange([FromBody] ChallangeRequestBody body)
    {
        if (body == null /*|| string.IsNullOrEmpty(body.Recaptcha)*/)
        {
            _logger.LogWarning("User submitted invalid Recaptcha Code", LoggerCategories.Login.ToString(), new Dictionary<string, string>()
            {
                {
                    "Content", JsonConvert.SerializeObject(body)
                }
            });

            return BadRequest("Common/GoogleVertficationFailed".AsTranslation());
        }

        //var mess = GoogleReCapcha.Validate(body.Recaptcha, Request.HttpContext.Connection.RemoteIpAddress.ToString());

        //if (!mess)
        //{
        //	return BadRequest("Recaptcha has Failed bacause: " + mess.Reason);
        //}

        using var db = _dbContextFactory.CreateDbContext();
        var user = db.AppUsers.Where(f => f.Username == body.Username).FirstOrDefault();
        if (user == null)
        {
            ModelState.AddModelError(nameof(ChallangeRequestBody.Username), "Unkown username");
            return BadRequest(ModelState);
        }

        string challange = null;
        challange = LoginChallangeManager.GetNew(body.Username, user.PasswordHash);

        if (challange == null)
        {
            return BadRequest("Login/MaxTrys".AsTranslation());
        }

        return Data(challange);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginFormModel loginModel)
    {
        var passwordSignInAsync = await _signInManager.PasswordSignInAsync(loginModel.Account, loginModel.Password, true, false);
        if (passwordSignInAsync.Succeeded)
        {
            var result = new LoginResult();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Value.Key));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            //var user = _db.AppUsers
            //	.Where
            //	.Column(f => f.NormUsername).Is.EqualsTo(loginModel.Username.ToUpper())
            //	.FirstOrDefault();

            var user = await _userStore.FindByNameAsync(loginModel.Account.ToUpper(), CancellationToken.None);
            var expireDate = DateTimeOffset.UtcNow.AddMonths(1);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.AppUserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Expiration, expireDate.ToString("O")),
                new Claim(Claims.Issued, DateTimeOffset.UtcNow.ToString("O")),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = new JwtSecurityToken(
                issuer: _tokenSettings.Value.Issuer,
                audience: _tokenSettings.Value.Audience,
                expires: expireDate.DateTime,
                signingCredentials: credentials,
                claims: claims
            );
            string token = JwtCoder.EncodeToken(jwtToken);
            result.Token = token;

            return Data(result);
        }

        return Unauthorized();
    }

    [HttpPost]
    [RevokableAuthorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        using var db = _dbContextFactory.CreateDbContext();
        var user = db.AppUsers.Find(User.GetUserId());
        if (user != null && user.IsTestUser)
        {
            Task.Run(() => AccountHelper.DeleteUser(user, db, _blobManagerService));
        }

        return Data();
    }

    [HttpPost]
    [RevokableAuthorize]
    [Route("LogoutEverywhere")]
    public async Task<IActionResult> LogoutEverywhere()
    {
        LogoutUserEverywhere(User.GetUserId());
        await _signInManager.SignOutAsync();
        return Data();
    }

    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("LogoutUserEverywhere")]
    public IActionResult LogoutUserEverywhere(Guid userId)
    {
        _tokenManager.LogoutUser(userId);
        return Data();
    }
}