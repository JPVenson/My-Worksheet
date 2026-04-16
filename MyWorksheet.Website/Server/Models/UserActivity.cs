using System;

namespace MyWorksheet.Website.Server.Models;

public partial class UserActivity : IUserRelation
{
    public Guid UserActivityId { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime DateCreated { get; set; }

    public Guid? IdCreator { get; set; }

    public string HeaderHtml { get; set; }

    public string BodyHtml { get; set; }

    public string FooterHtml { get; set; }

    public string ActivityType { get; set; }

    public string SystemActivityTypeKey { get; set; }

    public bool Hidden { get; set; }

    public bool Activated { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }
}
