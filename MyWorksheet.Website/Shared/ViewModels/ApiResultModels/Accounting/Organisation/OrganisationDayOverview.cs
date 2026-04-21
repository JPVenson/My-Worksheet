using System.Collections.Generic;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;

public class OrganisationDayOverview : ViewModelBase
{
    private AccountApiUserGetInfo[] _appUsers;

    private ICollection<GetProjectModel> _projects;

    private ICollection<WorksheetItemModel> _worksheetItems;

    private ICollection<WorksheetModel> _worksheets;

    public ICollection<GetProjectModel> Projects
    {
        get { return _projects; }
        set { SetProperty(ref _projects, value); }
    }

    public ICollection<WorksheetModel> Worksheets
    {
        get { return _worksheets; }
        set { SetProperty(ref _worksheets, value); }
    }

    public ICollection<WorksheetItemModel> WorksheetItems
    {
        get { return _worksheetItems; }
        set { SetProperty(ref _worksheetItems, value); }
    }

    public AccountApiUserGetInfo[] AppUsers
    {
        get { return _appUsers; }
        set { SetProperty(ref _appUsers, value); }
    }
}