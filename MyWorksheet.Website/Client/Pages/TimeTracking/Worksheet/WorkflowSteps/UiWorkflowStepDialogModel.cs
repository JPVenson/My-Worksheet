using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Dialog;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.WorkflowSteps;

public class UiWorkflowStepDialogModel : DialogViewModelBase
{
    private readonly WorksheetModel _worksheet;
    private readonly GetProjectModel _projectModel;

    public UiWorkflowStepDialogModel(WorksheetModel worksheet,
        GetProjectModel projectModel,
        WorksheetWorkflowStepViewModel nextStatus,
        IObjectSchemaInfo objectSchemaInfo)
    {
        _worksheet = worksheet;
        _projectModel = projectModel;
        NextStatus = nextStatus;
        Schema = objectSchemaInfo;
        Values = new ValueBag();
        FieldContext = new EditContext(this);
    }

    public WorksheetWorkflowStepViewModel NextStatus { get; }

    public WorksheetModel Worksheet
    {
        get
        {
            return _worksheet;
        }
    }

    public GetProjectModel Project
    {
        get
        {
            return _projectModel;
        }
    }

    public string SubmitReason { get; set; }

    public IObjectSchemaInfo Schema { get; set; }
    public ValueBag Values { get; set; }
    public EditContext FieldContext { get; set; }

    public bool WithStatus { get; set; }

    public async Task UpdateStatus()
    {
        WithStatus = true;
        await Close();
    }
}