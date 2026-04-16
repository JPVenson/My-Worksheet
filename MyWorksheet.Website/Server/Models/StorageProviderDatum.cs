using System;
namespace MyWorksheet.Website.Server.Models;

public partial class StorageProviderData : IUserRelation
{
    public Guid StorageProviderDataId { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid IdStorageProvider { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual StorageProvider IdStorageProviderNavigation { get; set; }
}
