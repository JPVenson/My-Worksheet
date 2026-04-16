using System.Security.Claims;

namespace MyWorksheet.Website.Server.Shared.Auth;

public static class Claims
{
    public static string UserClaimId { get; } = ClaimTypes.NameIdentifier; // "application.user.idClaim";
    public static string UserPersistClaimId { get; } = "application.user.persist";
    public static string ServerExpiresClaimId { get; } = "application.user.default.expire";
    public static string ServerRefreshExpiresClaimId { get; } = "application.user.default.refresh.expire";
    public static string ServerPasswordClaimId { get; } = "application.user.default.password";

    public static string Issued { get; } = "application.user.IssuedDate";
    public static string PasswordTokenClaim { get; private set; } = "application.user.default.pwguid";
    public static string LoginType { get; private set; } = "application.user.default.logintype";
    public static string UserToken { get; set; } = "userToken";
    public static string AutoToken { get; set; } = "autoToken";
}