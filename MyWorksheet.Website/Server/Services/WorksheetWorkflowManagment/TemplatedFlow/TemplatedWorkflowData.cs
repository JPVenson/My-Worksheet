using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.TemplatedFlow;

public class TemplatedWorkflowData
{
    [JsonComment("Workflow.Templated/Arguments.Comments.Created")]
    [JsonDisplayKey("Workflow.Templated/Arguments.Names.Created")]
    public int? ReportTemplateOnCreated { get; set; }

    [JsonComment("Workflow.Templated/Arguments.Comments.Submitted")]
    [JsonDisplayKey("Workflow.Templated/Arguments.Names.Submitted")]
    public int? ReportTemplateOnSubmitted { get; set; }

    [JsonComment("Workflow.Templated/Arguments.Comments.AwaitingResponse")]
    [JsonDisplayKey("Workflow.Templated/Arguments.Names.AwaitingResponse")]
    public int? ReportTemplateOnAwaitingResponse { get; set; }

    [JsonComment("Workflow.Templated/Arguments.Comments.Confirmed")]
    [JsonDisplayKey("Workflow.Templated/Arguments.Names.Confirmed")]
    public int? ReportTemplateOnConfirmed { get; set; }

    [JsonComment("Workflow.Templated/Arguments.Comments.AwaitingPayment")]
    [JsonDisplayKey("Workflow.Templated/Arguments.Names.AwaitingPayment")]
    public int? ReportTemplateOnAwaitingPayment { get; set; }

    [JsonComment("Workflow.Mail/Arguments.Comments.Storage")]
    [JsonDisplayKey("Workflow.Mail/Arguments.Names.Storage")]
    public string StorageProvider { get; set; }
}