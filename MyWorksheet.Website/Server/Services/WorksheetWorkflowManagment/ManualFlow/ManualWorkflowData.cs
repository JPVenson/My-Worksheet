using System;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Services.Reporting.Models;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.ManualFlow;

public class ManualWorkflowData
{
    [JsonComment("Workflow.Manual/Arguments.Comments.CreateReport")]
    [JsonDisplayKey("Workflow.Manual/Arguments.Names.CreateReport")]
    public bool CreateReport { get; set; }

    [JsonComment("Workflow.Manual/Arguments.Comments.SubmitReport")]
    [JsonDisplayKey("Workflow.Manual/Arguments.Names.SubmitReport")]
    public ManualWorkflowReport SubmitReport { get; set; }

    [JsonComment("Workflow.Manual/Arguments.Comments.ConfirmedReport")]
    [JsonDisplayKey("Workflow.Manual/Arguments.Names.ConfirmedReport")]
    public ManualWorkflowReport ConfirmedReport { get; set; }

    //[JsonComment("Can be used to Define a report that will be created when switching to Payed")]
    //public ManualWorkflowReport PayedReport { get; set; }

    public class ManualWorkflowReport
    {
        [JsonComment("Workflow.Manual/Arguments.Comments.Report.Storage")]
        [JsonDisplayKey("Workflow.Manual/Arguments.Names.Report.Storage")]
        public Guid StorageProvider { get; set; }

        [JsonComment("Workflow.Manual/Arguments.Comments.Report.Template")]
        [JsonDisplayKey("Workflow.Manual/Arguments.Names.Report.Template")]
        public Guid Template { get; set; }

        public WorkflowReportArguments ReportData { get; set; }
    }
}