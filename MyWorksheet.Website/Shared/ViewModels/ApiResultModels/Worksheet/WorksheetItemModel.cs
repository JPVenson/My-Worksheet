using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

[ObjectTracking("WorksheetItem")]
public class WorksheetItemModel : ViewModelBase
{
    private string _comment;
    private DateTimeOffset _dateOfAction;
    private int _fromTime;
    private Guid _id_Worksheet;
    private Guid _idProjectItemRate;
    private int _toTime;
    private Guid _worksheetItem_Id;

    public Guid WorksheetItemId
    {
        get { return _worksheetItem_Id; }
        set { SetProperty(ref _worksheetItem_Id, value); }
    }

    public Guid IdWorksheet
    {
        get { return _id_Worksheet; }
        set { SetProperty(ref _id_Worksheet, value); }
    }

    public DateTimeOffset DateOfAction
    {
        get { return _dateOfAction; }
        set { SetProperty(ref _dateOfAction, value); }
    }

    public int FromTime
    {
        get { return _fromTime; }
        set { SetProperty(ref _fromTime, value); }
    }

    public int ToTime
    {
        get { return _toTime; }
        set { SetProperty(ref _toTime, value); }
    }

    public string Comment
    {
        get { return _comment; }
        set { SetProperty(ref _comment, value); }
    }

    public Guid IdProjectItemRate
    {
        get { return _idProjectItemRate; }
        set { SetProperty(ref _idProjectItemRate, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetItemId;
    }
}