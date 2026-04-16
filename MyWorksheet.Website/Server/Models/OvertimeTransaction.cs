using System;

namespace MyWorksheet.Website.Server.Models;

public partial class OvertimeTransaction
{
    public Guid OvertimeTransactionId { get; set; }

    public bool Withdraw { get; set; }

    public decimal Value { get; set; }

    public DateTimeOffset DateOfAction { get; set; }

    public short DateOfActionOffset { get; set; }

    public Guid IdOvertimeAccount { get; set; }

    public virtual OvertimeAccount IdOvertimeAccountNavigation { get; set; }
}
