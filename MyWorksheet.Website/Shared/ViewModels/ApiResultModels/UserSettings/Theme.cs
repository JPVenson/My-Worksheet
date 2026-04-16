namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings
{
    public class Theme : ViewModelBase
    {
        private string _category;

        private string _name;

        private string _themeUrl;

        public string ThemeUrl
        {
            get { return _themeUrl; }
            set { SetProperty(ref _themeUrl, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
    }
}