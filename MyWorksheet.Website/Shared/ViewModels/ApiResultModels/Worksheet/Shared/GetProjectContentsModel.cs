namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Shared
{
    public class GetProjectContentsModel : ViewModelBase
    {
        private GetProjectModel _project;

        private WorksheetModel[] _worksheets;

        public GetProjectModel Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }

        public WorksheetModel[] Worksheets
        {
            get { return _worksheets; }
            set { SetProperty(ref _worksheets, value); }
        }
    }
}