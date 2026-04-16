using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage
{
    [ObjectTracking("StorageEntry")]
    public class StorageEntryViewModel : ViewModelBase
    {
        public Guid StorageEntryId { get; set; }

        public Guid IdStorageType { get; set; }

        public Guid IdStorageProvider { get; set; }
        public bool IsDeleted { get; set; }

        public string StorageKey { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public override Guid? GetModelIdentifier()
        {
            return StorageEntryId;
        }
    }
}