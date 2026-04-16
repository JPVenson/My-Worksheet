namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.Server
{
    public class ServerCapability : ViewModelBase
    {
        private string _name;

        private string _value;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
    }
}