using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Server.Models;

public partial class ProjectsInOrganisation
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

    public Guid? IdAppUser { get; set; }

    // public string IdRelations { get; set; }

    public IEnumerable<Guid> Relations { get; set; }

    public bool? NoModifications { get; set; }

    public static IQueryable<ProjectsInOrganisation> Query(MyworksheetContext db, Guid idAppUser)
    {
        return db.Projects
            .Select(e => new ProjectsInOrganisation()
            {
                BookToOvertimeAccount = e.BookToOvertimeAccount,
                Hidden = e.Hidden,
                IdAppUser = e.IdCreator,
                IdBillingFrame = e.IdBillingFrame,
                IdCreator = e.IdCreator,
                IdDefaultRate = e.IdDefaultRate,
                IdOrganisation = e.IdOrganisation,
                IdPaymentCondition = e.IdPaymentCondition,
                IdWorksheetWorkflow = e.IdWorksheetWorkflow,
                IdWorksheetWorkflowDataMap = e.IdWorksheetWorkflowDataMap,
                Name = e.Name,
                ProjectId = e.ProjectId,
                NumberRangeEntry = e.NumberRangeEntry,
                ProjectReference = e.ProjectReference,
                UserOrderNo = e.UserOrderNo,
                NoModifications = e.Worksheets.Any(f => f.IdCurrentStatusNavigation.AllowModifications == false),
                Relations = e.IdOrganisationNavigation.OrganisationUserMaps.Where(e => e.IdAppUser == idAppUser).Select(f => f.IdRelation)
            });
    }




    // public virtual Project Project { get; set; }
    // public virtual WorksheetWorkflow WorksheetWorkflow { get; set; }

    // public virtual WorksheetWorkflowDataMap WorksheetWorkflowDataMap { get; set; }
    // public virtual PaymentInfoContent PaymentCondition { get; set; }
    // public virtual BillingFrameLookup BillingFrame { get; set; }
    // public virtual Organisation Organisation { get; set; }
    // public virtual AppUser Creator { get; set; }
    // public virtual AppUser AppUser { get; set; }
}
