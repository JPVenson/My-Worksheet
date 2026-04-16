using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage
{
    public class GetStorageProvider : StorageProviderViewModel
    {
        private Guid? _idAppUser;

        private Guid _storageProviderId;

        public Guid StorageProviderId
        {
            get { return _storageProviderId; }
            set { SetProperty(ref _storageProviderId, value); }
        }

        public Guid? IdAppUser
        {
            get { return _idAppUser; }
            set { SetProperty(ref _idAppUser, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return StorageProviderId;
        }
    }
}