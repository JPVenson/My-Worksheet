namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage
{
    public class StorageProviderViewModel : ViewModelBase
    {
        private bool _isDefaultProvider;

        private string _name;

        private string _storageKey;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string StorageKey
        {
            get { return _storageKey; }
            set { SetProperty(ref _storageKey, value); }
        }

        public bool IsDefaultProvider
        {
            get { return _isDefaultProvider; }
            set { SetProperty(ref _isDefaultProvider, value); }
        }
    }
}