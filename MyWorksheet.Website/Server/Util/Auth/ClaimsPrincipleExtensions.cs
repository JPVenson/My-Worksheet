using System;
using System.Security.Claims;

namespace MyWorksheet.Website.Server.Util.Auth;

public static class ClaimsPrincipleExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
    public static Guid GetUserId(this ClaimsIdentity claimsPrincipal)
    {
        return Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
