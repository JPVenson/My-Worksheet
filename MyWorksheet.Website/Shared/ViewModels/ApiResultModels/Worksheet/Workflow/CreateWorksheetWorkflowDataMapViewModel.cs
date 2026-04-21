using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

public class CreateWorksheetWorkflowDataMapViewModel : ViewModelBase
{
    private IDictionary<string, object> _fields;

    private string _groupKey;

    private Guid? _idSharedWithOrganisation;

    private Guid _idWorksheetWorkflow;

    public Guid IdWorksheetWorkflow
    {
        get { return _idWorksheetWorkflow; }
        set { SetProperty(ref _idWorksheetWorkflow, value); }
    }

    public string GroupKey
    {
        get { return _groupKey; }
        set { SetProperty(ref _groupKey, value); }
    }

    public Guid? IdSharedWithOrganisation
    {
        get { return _idSharedWithOrganisation; }
        set { SetProperty(ref _idSharedWithOrganisation, value); }
    }

    public IDictionary<string, object> Fields
    {
        get { return _fields; }
        set { SetProperty(ref _fields, value); }
    }
}