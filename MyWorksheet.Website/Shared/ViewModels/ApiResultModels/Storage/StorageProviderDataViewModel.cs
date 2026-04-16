using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage
{
    public class StorageProviderDataViewModel : ViewModelBase
    {
        private string _key;

        private Guid _storageProviderDataId;

        private string _value;

        public Guid StorageProviderDataId
        {
            get { return _storageProviderDataId; }
            set { SetProperty(ref _storageProviderDataId, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return StorageProviderDataId;
        }
    }
}