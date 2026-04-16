using System;
using System.Collections.Concurrent;
using MyWorksheet.Website.Shared.Util;
using Newtonsoft.Json;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Auth;

[SingletonService()]
[WithAutoRegistration(RegistrationKind = RegistrationKind.All)]
public class ResetPasswordManager : IResetPasswordManager
{
    public ResetPasswordManager()
    {
        PasswordResetRequests = new ConcurrentDictionary<Guid, PasswordResetRequestModel>();
    }

    public ConcurrentDictionary<Guid, PasswordResetRequestModel> PasswordResetRequests { get; private set; }

    public static string PasswordHash { get; set; } = "EFB63A77-B2B3-4D3F-9D51-5256942EBFC3";

    public string IssueResetToken(Guid userId, string userName, string eMail)
    {
        var tokenList = PasswordResetRequests.GetOrAdd(userId, f => new PasswordResetRequestModel(f));
        return tokenList.IssueToken(userId, userName, eMail);
    }

    public bool RedeemPasswordToken(string token, out Guid userId)
    {
        try
        {
            var tokenJson = ChallangeUtil.DecryptPassword(token, PasswordHash);
            if (string.IsNullOrWhiteSpace(tokenJson))
            {
                userId = Guid.Empty;
                return false;
            }

            var passwordResetToken = JsonConvert.DeserializeObject<PasswordResetToken>(tokenJson);
            if (passwordResetToken != null)
            {
                userId = passwordResetToken.UserId;
                return PasswordResetRequests.GetOrAdd(passwordResetToken.UserId, f => new PasswordResetRequestModel(f))
                    .RedeemToken(passwordResetToken);
            }

            userId = Guid.Empty;
            return false;
        }
        catch (Exception)
        {
            userId = Guid.Empty;
            return false;
        }
    }
}