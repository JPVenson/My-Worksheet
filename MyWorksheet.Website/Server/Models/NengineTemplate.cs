using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class NengineTemplate : IUserRelation
{
    public Guid NengineTemplateId { get; set; }

    public string Template { get; set; }

    public string Purpose { get; set; }

    public string Name { get; set; }

    public string FileNameTemplate { get; set; }

    public string Comment { get; set; }

    public string UsedDataSource { get; set; }

    public string UsedFormattingEngine { get; set; }

    public string FileExtention { get; set; }

    public Guid? IdCreator { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual ICollection<NengineRunningTask> NengineRunningTasks { get; set; } = [];
}
