using System;
using System.Text;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Util;
using Newtonsoft.Json;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow.TokenManager;

[SingletonService(typeof(IEMailWorkflowTokenManager))]
public class EMailWorkflowTokenManager : IEMailWorkflowTokenManager
{
    public static string PasswordHash { get; set; } = "EFB63A77-B2B3-4D3F-9D51-5256942EBFC3";
    public string Encode(EMailWorkflowExternalToken token)
    {
        return Encoding.UTF8.GetBytes(ChallangeUtil.EncryptPassword(JsonConvert.SerializeObject(token), PasswordHash)).ToDecHex();
    }

    public EMailWorkflowExternalToken Decode(string text)
    {
        try
        {
            var eMailWorkflowExternalToken = JsonConvert.DeserializeObject<EMailWorkflowExternalToken>(
                ChallangeUtil.DecryptPassword(Encoding.UTF8.GetString(ChallangeUtil.StringToByteArrayFastest(text)),
                    PasswordHash));
            if (eMailWorkflowExternalToken?.ValidUntil < DateTimeOffset.UtcNow)
            {
                return null;
            }
            return eMailWorkflowExternalToken;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}

public class EMailWorkflowExternalToken
{
    public Guid UserId { get; set; }
    public Guid WorksheetId { get; set; }
    public DateTimeOffset ValidUntil { get; set; }
    public string Signer { get; set; }
}