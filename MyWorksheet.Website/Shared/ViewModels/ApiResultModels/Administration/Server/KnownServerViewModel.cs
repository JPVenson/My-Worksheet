namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.Server
{
    public class KnownServerViewModel : ViewModelBase
    {
        private string _hostName;

        private string _name;

        private bool _online;

        private ServerCapability[] _serverCapabilities;

        private string _type;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string HostName
        {
            get { return _hostName; }
            set { SetProperty(ref _hostName, value); }
        }

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public bool Online
        {
            get { return _online; }
            set { SetProperty(ref _online, value); }
        }

        public ServerCapability[] ServerCapabilities
        {
            get { return _serverCapabilities; }
            set { SetProperty(ref _serverCapabilities, value); }
        }
    }
}