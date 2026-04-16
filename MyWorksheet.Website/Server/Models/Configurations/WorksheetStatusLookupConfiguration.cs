using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class WorksheetStatusLookupConfiguration : IEntityTypeConfiguration<WorksheetStatusLookup>
{
    public static readonly WorksheetStatusLookup Invalid = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000001"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.Invalid", AllowModifications = false };
    public static readonly WorksheetStatusLookup Created = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000002"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.Created", AllowModifications = true };
    public static readonly WorksheetStatusLookup Submitted = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000003"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.Submitted", AllowModifications = false };
    public static readonly WorksheetStatusLookup AwaitingResponse = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000004"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.AwaitingResponse", AllowModifications = false };
    public static readonly WorksheetStatusLookup Confirmed = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000006"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.Confirmed", AllowModifications = false };
    public static readonly WorksheetStatusLookup AwaitingPayment = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000007"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.AwaitingPayment", AllowModifications = false };
    public static readonly WorksheetStatusLookup Rejected = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000008"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.Rejected", AllowModifications = false };
    public static readonly WorksheetStatusLookup Payed = new WorksheetStatusLookup() { WorksheetStatusLookupId = new Guid("00000000-0000-0000-0000-000000000009"), DescriptionKey = "", DisplayKey = "Workflow.Manual/StatusType.Payed", AllowModifications = false };

    public void Configure(EntityTypeBuilder<WorksheetStatusLookup> entity)
    {
        entity.HasKey(e => e.WorksheetStatusLookupId).HasName("PK__Workshee__BB44935C5AAACAC9");

        entity.ToTable("WorksheetStatusLookup");

        entity.Property(e => e.WorksheetStatusLookupId).HasColumnName("WorksheetStatusLookup_Id");
        entity.Property(e => e.DescriptionKey)
            .IsRequired()
            .HasMaxLength(350);
        entity.Property(e => e.DisplayKey)
            .IsRequired()
            .HasMaxLength(250);

        entity.HasData(Invalid, Created, Submitted, AwaitingResponse, Confirmed, AwaitingPayment, Rejected, Payed);
    }
}
