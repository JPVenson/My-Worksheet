using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;

public class WorksheetAssertCreateViewModel : WorksheetAssertGetViewModel
{
    private Guid? _idProject;

    private Guid? _idWorksheet;

    public Guid? IdWorksheet
    {
        get { return _idWorksheet; }
        set { SetProperty(ref _idWorksheet, value); }
    }

    public Guid? IdProject
    {
        get { return _idProject; }
        set { SetProperty(ref _idProject, value); }
    }
}