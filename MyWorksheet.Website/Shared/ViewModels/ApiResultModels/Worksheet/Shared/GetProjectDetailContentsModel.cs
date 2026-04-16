namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Shared
{
    public class GetProjectDetailContentsModel : ViewModelBase
    {
        private GetProjectModel _project;

        private WorksheetModel _worksheet;

        private WorksheetItemModel[] _worksheetItems;

        public GetProjectModel Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }

        public WorksheetModel Worksheet
        {
            get { return _worksheet; }
            set { SetProperty(ref _worksheet, value); }
        }

        public WorksheetItemModel[] WorksheetItems
        {
            get { return _worksheetItems; }
            set { SetProperty(ref _worksheetItems, value); }
        }
    }
}