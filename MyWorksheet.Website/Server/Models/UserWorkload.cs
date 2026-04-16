using System;
namespace MyWorksheet.Website.Server.Models;

public partial class UserWorkload : IUserRelation
{
    public Guid UserWorkloadId { get; set; }

    public int WorkTimeMode { get; set; }

    public decimal? DayWorkTimeMonday { get; set; }

    public decimal? DayWorkTimeTuesday { get; set; }

    public decimal? DayWorkTimeWednesday { get; set; }

    public decimal? DayWorkTimeThursday { get; set; }

    public decimal? DayWorkTimeFriday { get; set; }

    public decimal? DayWorkTimeSaturday { get; set; }

    public decimal? DayWorkTimeSunday { get; set; }

    public decimal WeekWorktime { get; set; }

    public decimal MonthWorktime { get; set; }

    public Guid? IdProject { get; set; }

    public Guid? IdOrganisation { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Organisation IdOrganisationNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }
}
