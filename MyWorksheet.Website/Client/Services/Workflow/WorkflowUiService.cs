using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Services.Workflow.Types;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Workflow;

[SingletonService()]
public class WorkflowUiService
{
    private readonly WaiterService _waiterService;

    public WorkflowUiService(WaiterService waiterService,
                             ActivatorService activatorService)
    {
        _waiterService = waiterService;
        WorkflowClientHandlers = new Dictionary<Guid, IWorkflowClientHandler>();
        DefaultWorkflowClientHandler = activatorService.ActivateType<DefaultWorkflowClientHandler>();
    }

    public IDictionary<Guid, IWorkflowClientHandler> WorkflowClientHandlers { get; set; }
    private IWorkflowClientHandler DefaultWorkflowClientHandler { get; }

    public async Task<(bool, Guid)> StartChangeWorkflowStep(Guid worksheetId, Guid workflowStepId, Guid workflowId, ServerErrorManager serverErrorManager)
    {
        if (!WorkflowClientHandlers.TryGetValue(workflowId, out var clientHandler))
        {
            clientHandler = DefaultWorkflowClientHandler;
        }

        using (_waiterService.WhenDisposed())
        {
            return await clientHandler.OnStatusChange(worksheetId, workflowStepId, serverErrorManager);
        }
    }
}
