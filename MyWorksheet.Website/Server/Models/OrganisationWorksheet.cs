using System;
using System.Linq;

namespace MyWorksheet.Website.Server.Models;

public partial class OrganisationWorksheet
{
    public Guid OrganisationId { get; set; }

    public virtual Organisation Organisation { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser AppUser { get; set; }

    public bool IsActive { get; set; }

    public virtual IQueryable<Worksheet> Worksheets { get; set; }

    // public string WorksheetXml { get; set; }
}
