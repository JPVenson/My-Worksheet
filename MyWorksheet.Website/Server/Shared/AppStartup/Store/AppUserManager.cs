using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Util.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Shared.AppStartup.Store;

public class AppUserManager : UserManager<AppUser>
{
    public AppUserManager(IUserStore<AppUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }

    protected override async Task<PasswordVerificationResult> VerifyPasswordAsync(IUserPasswordStore<AppUser> store, AppUser user, string password)
    {
        var challangeResult = LoginChallangeManager.CheckChallange(user.Username, password);
        if (challangeResult.Success)
        {
            return PasswordVerificationResult.Success;
        }

        return PasswordVerificationResult.Failed;
    }
}