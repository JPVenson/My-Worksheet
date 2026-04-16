using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ClientStructureWithRight
{
    public int ClientStructureId { get; set; }

    public bool MenuItemOnly { get; set; }

    public bool CanBeDirectlyNavigated { get; set; }

    public int? ParentRoute { get; set; }

    public string DisplayRoute { get; set; }

    public string AdditonalInfos { get; set; }

    public string ControllerName { get; set; }

    public string UrlRoute { get; set; }

    public string Title { get; set; }

    public int OrderId { get; set; }

    public bool IsActive { get; set; }

    public string InActiveNotice { get; set; }

    public bool Inverse { get; set; }

    public Guid IdRole { get; set; }

    public virtual Role Role { get; set; }
}
