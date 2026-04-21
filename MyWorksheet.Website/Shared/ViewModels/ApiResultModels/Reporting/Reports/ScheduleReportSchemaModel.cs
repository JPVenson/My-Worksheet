using MyWorksheet.Private.Models.ObjectSchema;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

public class ScheduleReportSchemaModel : ViewModelBase
{
    private JsonSchema _argumentSchema;

    private JsonSchema _querySchema;

    public JsonSchema QuerySchema
    {
        get { return _querySchema; }
        set { SetProperty(ref _querySchema, value); }
    }

    public JsonSchema ArgumentSchema
    {
        get { return _argumentSchema; }
        set { SetProperty(ref _argumentSchema, value); }
    }
}