namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow.TokenManager;

public interface IEMailWorkflowTokenManager
{
    string Encode(EMailWorkflowExternalToken token);
    EMailWorkflowExternalToken Decode(string text);
}
