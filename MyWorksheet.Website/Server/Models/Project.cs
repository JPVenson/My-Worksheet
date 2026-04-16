using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Project : IUserRelation
{
    public Guid ProjectId { get; set; }

    public string Name { get; set; }

    public bool Hidden { get; set; }

    public bool BookToOvertimeAccount { get; set; }

    public Guid? IdWorksheetWorkflow { get; set; }

    public Guid? IdWorksheetWorkflowDataMap { get; set; }

    public int UserOrderNo { get; set; }

    public string NumberRangeEntry { get; set; }

    public string ProjectReference { get; set; }

    public Guid? IdPaymentCondition { get; set; }

    public Guid? IdBillingFrame { get; set; }

    public Guid? IdOrganisation { get; set; }

    public Guid IdCreator { get; set; }

    public Guid? IdDefaultRate { get; set; }

    public virtual BillingFrameLookup IdBillingFrameNavigation { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual ProjectItemRate IdDefaultRateNavigation { get; set; }

    public virtual Organisation IdOrganisationNavigation { get; set; }

    public virtual PaymentInfo IdPaymentConditionNavigation { get; set; }

    public virtual WorksheetWorkflowDataMap IdWorksheetWorkflowDataMapNavigation { get; set; }

    public virtual WorksheetWorkflow IdWorksheetWorkflowNavigation { get; set; }

    public virtual ICollection<OvertimeAccount> OvertimeAccounts { get; set; } = [];

    public virtual ICollection<ProjectAddressMap> ProjectAddressMaps { get; set; } = [];

    public virtual ICollection<ProjectAssosiatedUserMap> ProjectAssosiatedUserMaps { get; set; } = [];

    public virtual ICollection<ProjectBudget> ProjectBudgets { get; set; } = [];

    public virtual ICollection<ProjectItemRate> ProjectItemRates { get; set; } = [];

    public virtual ICollection<ProjectShareKey> ProjectShareKeys { get; set; } = [];

    public virtual ICollection<UserWorkload> UserWorkloads { get; set; } = [];

    public virtual ICollection<WorksheetAssert> WorksheetAsserts { get; set; } = [];

    public virtual ICollection<Worksheet> Worksheets { get; set; } = [];
}
