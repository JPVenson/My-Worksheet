using System;

namespace MyWorksheet.Website.Server.Services.Auth;

public interface IResetPasswordManager
{
    string IssueResetToken(Guid userId, string userName, string eMail);
    bool RedeemPasswordToken(string token, out Guid userId);
}