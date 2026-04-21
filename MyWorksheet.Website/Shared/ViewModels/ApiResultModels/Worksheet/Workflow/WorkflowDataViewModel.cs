using System.Collections.Generic;
using MyWorksheet.Private.Models.ObjectSchema;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

public class WorkflowDataViewModel : ViewModelBase
{
    private JsonSchema _objectSchema;

    private IDictionary<string, object> _values;

    public JsonSchema ObjectSchema
    {
        get { return _objectSchema; }
        set { SetProperty(ref _objectSchema, value); }
    }

    public IDictionary<string, object> Values
    {
        get { return _values; }
        set { SetProperty(ref _values, value); }
    }
}