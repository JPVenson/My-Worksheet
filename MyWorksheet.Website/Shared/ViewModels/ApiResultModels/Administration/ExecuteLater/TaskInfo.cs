namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater
{
    public class TaskInfo : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
    }
}