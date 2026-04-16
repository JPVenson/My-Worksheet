using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class OvertimeAccount : IUserRelation
{
    public Guid OvertimeAccountId { get; set; }

    public decimal OvertimeValue { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public Guid IdProject { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }

    public virtual ICollection<OvertimeTransaction> OvertimeTransactions { get; set; } = [];
}
