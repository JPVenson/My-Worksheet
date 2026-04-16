using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class ClientStructure
{
    public Guid ClientStructureId { get; set; }

    public bool MenuItemOnly { get; set; }

    public bool CanBeDirectlyNavigated { get; set; }

    public Guid? ParentRoute { get; set; }

    public string DisplayRoute { get; set; }

    public string AdditonalInfos { get; set; }

    public string ControllerName { get; set; }

    public string UrlRoute { get; set; }

    public string Title { get; set; }

    public Guid OrderId { get; set; }

    public bool IsActive { get; set; }

    public string InActiveNotice { get; set; }

    public virtual ICollection<ClientSturctureRight> ClientSturctureRights { get; set; } = [];

    public virtual ICollection<ClientStructure> InverseParentRouteNavigation { get; set; } = [];

    public virtual ClientStructure ParentRouteNavigation { get; set; }
}
