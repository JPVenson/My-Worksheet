using System;

namespace MyWorksheet.Website.Server.Models;

public partial class ProjectBudget : IUserRelation
{
    public Guid ProjectBudgetId { get; set; }

    public Guid? IdAppUser { get; set; }

    public Guid IdProject { get; set; }

    public DateTimeOffset? Deadline { get; set; }

    public short? DeadlineOffset { get; set; }

    public int? TotalTimeBudget { get; set; }

    public decimal? TotalBudget { get; set; }

    public decimal TimeConsumed { get; set; }

    public decimal BugetConsumed { get; set; }

    public DateTimeOffset? ValidFrom { get; set; }

    public short? ValidFromOffset { get; set; }

    public bool AllowOverbooking { get; set; }

    public byte[] RowVersion { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }
}
