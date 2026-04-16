using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.WaiterIndicator;

namespace MyWorksheet.Website.Client.Services.Workflow.Types;

public interface IWorkflowClientHandler
{
    ValueTask<(bool, Guid)> OnStatusChange(Guid worksheetId, Guid workflowStep, ServerErrorManager errorManager);
}