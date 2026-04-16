using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetWorkflowData
{
    public Guid WorksheetWorkflowDataId { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public Guid IdWorksheetWorkflowMap { get; set; }

    public virtual WorksheetWorkflowDataMap IdWorksheetWorkflowMapNavigation { get; set; }
}
