using System.Security.Claims;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Shared.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers.Api;

public class PageSettingsApiControllerBase : ApiControllerBase
{
    public IActionResult Get()
    {
        var setting = new PageSetting();
        if (User != null)
        {
            if (User.Identity.IsAuthenticated)
            {
                setting.IsLoggedin = true;
            }
            if (User.IsInRole(Roles.AdminRoleName))
            {
                setting.IsAdmin = true;
            }

            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                if (identity.HasClaim(f => f.Type == Claims.UserPersistClaimId && f.Value == "True"))
                {
                    setting.Persist = false;
                }
                setting.SessionTimeout = identity.FindFirst(ClaimTypes.Expiration)?.Value;
                setting.IssuedDate = identity.FindFirst(Claims.Issued)?.Value;
            }
        }
        return Data(setting);
    }
}