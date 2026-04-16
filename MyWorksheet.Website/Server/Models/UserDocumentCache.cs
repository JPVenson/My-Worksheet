using System;

namespace MyWorksheet.Website.Server.Models;

public partial class UserDocumentCache : IUserRelation
{
    public Guid UserDocumentCacheId { get; set; }

    public string Name { get; set; }

    public string Link { get; set; }

    public Guid IdUser { get; set; }

    public string HostenOn { get; set; }

    public string FileType { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual AppUser IdUserNavigation { get; set; }
}
