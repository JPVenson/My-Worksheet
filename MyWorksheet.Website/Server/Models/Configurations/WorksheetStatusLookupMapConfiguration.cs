using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class WorksheetStatusLookupMapConfiguration : IEntityTypeConfiguration<WorksheetStatusLookupMap>
{
    public void Configure(EntityTypeBuilder<WorksheetStatusLookupMap> entity)
    {
        entity.HasKey(e => e.WorksheetStatusLookupMapId).HasName("PK__Workshee__CAD389088D1F4B8D");

        entity.ToTable("WorksheetStatusLookupMap");

        entity.Property(e => e.WorksheetStatusLookupMapId).HasColumnName("WorksheetStatusLookupMap_Id");
        entity.Property(e => e.IdFromStatus).HasColumnName("Id_FromStatus");
        entity.Property(e => e.IdToStatus).HasColumnName("Id_ToStatus");
        entity.Property(e => e.IdWorkflow).HasColumnName("Id_Workflow");

        entity.HasOne(d => d.IdFromStatusNavigation).WithMany(p => p.WorksheetStatusLookupMapIdFromStatusNavigations)
            .HasForeignKey(d => d.IdFromStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetStatusLookupMap_WorkflowStatusLookup_Parent");

        entity.HasOne(d => d.IdToStatusNavigation).WithMany(p => p.WorksheetStatusLookupMapIdToStatusNavigations)
            .HasForeignKey(d => d.IdToStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetStatusLookupMap_WorkflowStatusLookup_Child");

        entity.HasOne(d => d.IdWorkflowNavigation).WithMany(p => p.WorksheetStatusLookupMaps)
            .HasForeignKey(d => d.IdWorkflow)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetStatusLookupMap_WorksheetWorkflow");

        // Email workflow (workflow 2)
        //Invalid to created
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000001"), IdFromStatus = WorksheetStatusLookupConfiguration.Invalid.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.EmailWorkflow });
        //Created to AwaitingResponse
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000008"), IdFromStatus = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.AwaitingResponse.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.EmailWorkflow });
        //AwaitingResponse to Confirmed or Rejected
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000009"), IdFromStatus = WorksheetStatusLookupConfiguration.AwaitingResponse.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Confirmed.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.EmailWorkflow });
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-00000000000a"), IdFromStatus = WorksheetStatusLookupConfiguration.AwaitingResponse.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Rejected.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.EmailWorkflow });
        //Confirmed to AwaitingPayment
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-00000000000b"), IdFromStatus = WorksheetStatusLookupConfiguration.Confirmed.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.AwaitingPayment.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.EmailWorkflow });
        //AwaitingPayment to Payed
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-00000000000c"), IdFromStatus = WorksheetStatusLookupConfiguration.AwaitingPayment.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Payed.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.EmailWorkflow });

        // Manual workflow (workflow 1)
        //Invalid to created
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-00000000000d"), IdFromStatus = WorksheetStatusLookupConfiguration.Invalid.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
        //Created to Submitted
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-00000000000e"), IdFromStatus = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Submitted.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
        //Submitted to Confirmed or Rejected
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-00000000000f"), IdFromStatus = WorksheetStatusLookupConfiguration.Submitted.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Confirmed.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000010"), IdFromStatus = WorksheetStatusLookupConfiguration.Submitted.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Rejected.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
        //Confirmed to Payed or Rejected
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000011"), IdFromStatus = WorksheetStatusLookupConfiguration.Confirmed.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Payed.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000012"), IdFromStatus = WorksheetStatusLookupConfiguration.Confirmed.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Rejected.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
        //Rejected to created
        entity.HasData(new WorksheetStatusLookupMap() { WorksheetStatusLookupMapId = new Guid("00000000-0000-0000-0004-000000000013"), IdFromStatus = WorksheetStatusLookupConfiguration.Rejected.WorksheetStatusLookupId, IdToStatus = WorksheetStatusLookupConfiguration.Created.WorksheetStatusLookupId, IdWorkflow = WorksheetWorkflowConfiguration.ManualWorkflow });
    }
}
