using System;
namespace MyWorksheet.Website.Server.Models;

public partial class TextResource
{
    public Guid IdTextResource { get; set; }

    public string Text { get; set; }

    public string Lang { get; set; }

    public string Page { get; set; }

    public string Key { get; set; }
}
