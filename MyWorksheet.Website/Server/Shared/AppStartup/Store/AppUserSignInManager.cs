using MyWorksheet.Website.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Shared.AppStartup.Store;

public class AppUserSignInManager : SignInManager<AppUser>
{
    public AppUserSignInManager(UserManager<AppUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<AppUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<AppUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<AppUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }
}