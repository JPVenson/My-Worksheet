using MyWorksheet.Helper;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow;

public class EMailWorkflowData : ArgumentsBase
{
    [JsonComment("Workflow.Mail/Arguments.Comments.MailAccount")]
    [JsonDisplayKey("Workflow.Mail/Arguments.Names.MailAccount")]
    public int MailAccount { get; set; }

    [JsonComment("Workflow.Mail/Arguments.Comments.Storage")]
    [JsonDisplayKey("Workflow.Mail/Arguments.Names.Storage")]
    public string MailStorageProvider { get; set; }

    [JsonComment("Workflow.Mail/Arguments.Comments.AwaitingResponse")]
    [JsonDisplayKey("Workflow.Mail/Arguments.Names.AwaitingResponse")]
    public int ReportTemplateAwaitingResponse { get; set; }
}