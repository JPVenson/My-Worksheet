using System.Collections.Generic;
using MyWorksheet.Website.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MyWorksheet.Website.Server.Shared.AppStartup.Store;

public class AppRoleManager : RoleManager<Role>
{
    public AppRoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
    {

    }
}