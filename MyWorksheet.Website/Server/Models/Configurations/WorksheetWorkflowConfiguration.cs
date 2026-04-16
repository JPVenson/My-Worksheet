using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class WorksheetWorkflowConfiguration : IEntityTypeConfiguration<WorksheetWorkflow>
{
    // Workflow Guid constants
    public static readonly Guid ManualWorkflow = new Guid("00000000-0000-0000-0000-000000000001");
    public static readonly Guid EmailWorkflow = new Guid("00000000-0000-0000-0000-000000000002");

    public void Configure(EntityTypeBuilder<WorksheetWorkflow> entity)
    {
        entity.HasKey(e => e.WorksheetWorkflowId).HasName("PK__Workshee__1F09726A9D7BB59F");

        entity.ToTable("WorksheetWorkflow");

        entity.Property(e => e.WorksheetWorkflowId).HasColumnName("WorksheetWorkflow_Id");
        entity.Property(e => e.DisplayKey)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.IdDefaultStep).HasColumnName("Id_DefaultStep");
        entity.Property(e => e.NeedsCustomData).HasDefaultValue(false);
        entity.Property(e => e.ProviderKey).IsRequired();

        entity.HasOne(d => d.IdDefaultStepNavigation).WithMany(p => p.WorksheetWorkflowIdDefaultStepNavigations)
            .HasForeignKey(d => d.IdDefaultStep)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflow_DefaultStep");

        entity.HasData(new WorksheetWorkflow()
        {
            WorksheetWorkflowId = ManualWorkflow,
            DisplayKey = "Manual",
            Comment = "No Automatism. All states must be triggered manual on the Webpage",
            ProviderKey = "ManualWorkflowImpl",
            IdDefaultStep = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId,
            NeedsCustomData = false
        });
        entity.HasData(new WorksheetWorkflow()
        {
            WorksheetWorkflowId = EmailWorkflow,
            DisplayKey = "E-Mail Workflow",
            Comment = "Includes E-Mail to be send by an E-Mail provider of yours.",
            ProviderKey = "MailWorkflow",
            IdDefaultStep = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId,
            NeedsCustomData = true
        });
    }
}
