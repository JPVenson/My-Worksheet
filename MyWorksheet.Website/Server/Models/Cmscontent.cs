using System;
namespace MyWorksheet.Website.Server.Models;

public partial class Cmscontent
{
    public Guid CmscontnetId { get; set; }

    public string ContentId { get; set; }

    public string Content { get; set; }

    public string ContentLang { get; set; }

    public string ContentTemplate { get; set; }

    public bool IsJsonblob { get; set; }

    public bool RequireAuth { get; set; }
}
