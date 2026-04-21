using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class WorksheetItemStatusModel : ViewModelBase
{
    private string _comment;

    private int _dateOfAction;

    private int _fromTime;

    private int _id_Worksheet;

    private int _toTime;

    private Guid _worksheetItem_Id;

    public Guid WorksheetItem_Id
    {
        get { return _worksheetItem_Id; }
        set { SetProperty(ref _worksheetItem_Id, value); }
    }

    public int Id_Worksheet
    {
        get { return _id_Worksheet; }
        set { SetProperty(ref _id_Worksheet, value); }
    }

    public int DateOfAction
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

    public override Guid? GetModelIdentifier()
    {
        return WorksheetItem_Id;
    }
}