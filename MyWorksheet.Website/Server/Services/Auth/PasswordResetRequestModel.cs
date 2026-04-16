using System;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Shared.Util;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Services.Auth;

public class PasswordResetRequestModel
{
    public PasswordResetRequestModel(Guid userId)
    {
        UserId = userId;
        PasswordResetTokens = new ConcurrentHashSet<PasswordResetToken>(PasswordResetToken.PasswordResetTokenComparer);
    }

    public Guid UserId { get; set; }
    public ConcurrentHashSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public string IssueToken(Guid userId, string userName, string email)
    {
        PasswordResetTokens.RemoveWhere(e => e.ValidUntil < DateTime.UtcNow);
        if (PasswordResetTokens.Count > 5)
        {
            return null;
        }

        var token = new PasswordResetToken();
        token.ValidUntil = DateTime.UtcNow.AddDays(1);
        token.UserId = userId;
        token.Username = userName;
        token.Email = email;
        token.Token = ChallangeUtil.EncryptPassword(JsonConvert.SerializeObject(token), userName);
        PasswordResetTokens.TryAdd(token);
        return ChallangeUtil.EncryptPassword(JsonConvert.SerializeObject(token), ResetPasswordManager.PasswordHash);
    }

    public bool RedeemToken(PasswordResetToken passwordResetToken)
    {
        PasswordResetTokens.RemoveWhere(e => e.ValidUntil < DateTime.UtcNow);
        //var hasToken = PasswordResetTokens.Get(e => PasswordResetToken.PasswordResetTokenComparer.Equals(e, passwordResetToken));
        //if (hasToken == null)
        //{
        //	return false;
        //}

        return PasswordResetTokens.Remove(passwordResetToken);
    }
}