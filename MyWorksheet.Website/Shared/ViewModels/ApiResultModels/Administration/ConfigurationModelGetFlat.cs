namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration
{
    public class ConfigurationModelGetFlat : ViewModelBase
    {
        private string _path;

        private string _value;

        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
    }
}