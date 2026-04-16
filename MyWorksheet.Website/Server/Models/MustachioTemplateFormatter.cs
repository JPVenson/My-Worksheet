using System;
namespace MyWorksheet.Website.Server.Models;

public partial class MustachioTemplateFormatter : IUserRelation
{
    public Guid MustachioTemplateFormatterId { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public Guid IdCreator { get; set; }

    public Guid IdStorageEntry { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual StorageEntry IdStorageEntryNavigation { get; set; }
}
