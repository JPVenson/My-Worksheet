using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

public class WorksheetWorkflowStatusLookupViewModel : ViewModelBase
{
    private string _descriptionKey;

    private string _displayKey;

    private Guid _worksheetStatusLookupId;

    public bool AllowModifications { get; set; }

    public Guid WorksheetStatusLookupId
    {
        get { return _worksheetStatusLookupId; }
        set { SetProperty(ref _worksheetStatusLookupId, value); }
    }

    public string DisplayKey
    {
        get { return _displayKey; }
        set { SetProperty(ref _displayKey, value); }
    }

    public string DescriptionKey
    {
        get { return _descriptionKey; }
        set { SetProperty(ref _descriptionKey, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetStatusLookupId;
    }
}