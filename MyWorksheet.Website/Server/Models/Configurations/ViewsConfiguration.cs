using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class ClientStructureWithRightConfiguration : IEntityTypeConfiguration<ClientStructureWithRight>
{
    public void Configure(EntityTypeBuilder<ClientStructureWithRight> entity)
    {
        entity.HasNoKey().ToView("ClientStructureWithRights");

        entity.Property(e => e.ClientStructureId).HasColumnName("ClientStructure_Id");
        entity.Property(e => e.IdRole).HasColumnName("Id_Role");
    }
}

public class OrdersAggregatedConfiguration : IEntityTypeConfiguration<OrdersAggregated>
{
    public void Configure(EntityTypeBuilder<OrdersAggregated> entity)
    {
        entity.HasNoKey().ToView("OrdersAggregated");

        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdPromisedFeatureContent).HasColumnName("Id_PromisedFeatureContent");
    }
}

public class OrganisationWorksheetConfiguration : IEntityTypeConfiguration<OrganisationWorksheet>
{
    public void Configure(EntityTypeBuilder<OrganisationWorksheet> entity)
    {
        entity.HasNoKey().ToView("OrganisationWorksheets");

        entity.Property(e => e.OrganisationId).HasColumnName("Organisation_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");

        // Worksheets is a navigation collection resolved in application code, not a view column
        entity.Ignore(e => e.Worksheets);
    }
}

public class OrganisationUserMappingConfiguration : IEntityTypeConfiguration<OrganisationUserMapping>
{
    public void Configure(EntityTypeBuilder<OrganisationUserMapping> entity)
    {
        entity.HasNoKey().ToView("OrganisationUserMappings");

        entity.Property(e => e.OrganisationId).HasColumnName("Organisation_Id");
        entity.HasOne(e => e.Organisation)
            .WithOne()
            .HasForeignKey<OrganisationUserMapping>(e => e.OrganisationId)
            .HasPrincipalKey<Organisation>(e => e.OrganisationId);
        entity.Property(e => e.IdAddress).HasColumnName("Id_Address");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdParentOrganisation).HasColumnName("Id_ParentOrganisation");
        entity.HasOne(e => e.ParentOrganisation)
            .WithOne()
            .HasForeignKey<OrganisationUserMapping>(e => e.IdParentOrganisation)
            .HasPrincipalKey<Organisation>(e => e.OrganisationId);

        // IdRelations is a collection resolved in application code, not a view column
        entity.Ignore(e => e.IdRelations);
    }
}

public class PerDayReportingConfiguration : IEntityTypeConfiguration<PerDayReporting>
{
    public void Configure(EntityTypeBuilder<PerDayReporting> entity)
    {
        entity.HasNoKey().ToView("PerDayReporting");

        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");
        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdCurrentStatus).HasColumnName("Id_CurrentStatus");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");

        // WorksheetActionsCsv is a concatenated string produced by the view
    }
}

public class ProjectOverviewReportingConfiguration : IEntityTypeConfiguration<ProjectOverviewReporting>
{
    public void Configure(EntityTypeBuilder<ProjectOverviewReporting> entity)
    {
        entity.HasNoKey().ToView("ProjectOverviewReporting");

        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.ProjectId).HasColumnName("ProjectId");
    }
}

public class ProjectReportingConfiguration : IEntityTypeConfiguration<ProjectReporting>
{
    public void Configure(EntityTypeBuilder<ProjectReporting> entity)
    {
        entity.HasNoKey().ToView("ProjectReporting");

        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
    }
}

public class ProjectsInOrganisationConfiguration : IEntityTypeConfiguration<ProjectsInOrganisation>
{
    public void Configure(EntityTypeBuilder<ProjectsInOrganisation> entity)
    {
        entity.HasNoKey().ToView("ProjectsInOrganisation");

        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdWorksheetWorkflow).HasColumnName("Id_WorksheetWorkflow");
        entity.Property(e => e.IdWorksheetWorkflowDataMap).HasColumnName("Id_WorksheetWorkflowDataMap");
        entity.Property(e => e.IdPaymentCondition).HasColumnName("Id_PaymentCondition");
        entity.Property(e => e.IdBillingFrame).HasColumnName("Id_BillingFrame");
        entity.Property(e => e.IdOrganisation).HasColumnName("Id_Organisation");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdDefaultRate).HasColumnName("Id_DefaultRate");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");

        // Relations is a collection resolved in application code, not a view column
        entity.Ignore(e => e.Relations);
    }
}

public class SubmittedProjectConfiguration : IEntityTypeConfiguration<SubmittedProject>
{
    public void Configure(EntityTypeBuilder<SubmittedProject> entity)
    {
        entity.HasNoKey().ToView("SubmittedProjects");

        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.Wcount).HasColumnName("WCount");
    }
}

public class SubmittedWorksheetsReportingConfiguration : IEntityTypeConfiguration<SubmittedWorksheetsReporting>
{
    public void Configure(EntityTypeBuilder<SubmittedWorksheetsReporting> entity)
    {
        entity.HasNoKey().ToView("SubmittedWorksheetsReporting");

        entity.Property(e => e.WorksheetId).HasColumnName("Worksheet_Id");
        entity.Property(e => e.IdCurrentStatus).HasColumnName("Id_CurrentStatus");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdWorksheetWorkflow).HasColumnName("Id_WorksheetWorkflow");
        entity.Property(e => e.IdWorksheetWorkflowDataMap).HasColumnName("Id_WorksheetWorkflowDataMap");
    }
}

public class UserOrganisationMappingConfiguration : IEntityTypeConfiguration<UserOrganisationMapping>
{
    public void Configure(EntityTypeBuilder<UserOrganisationMapping> entity)
    {
        entity.HasNoKey().ToView("UserOrganisationMappings");

        entity.Property(e => e.AppUserId).HasColumnName("AppUser_Id");
        entity.Property(e => e.IdAddress).HasColumnName("Id_Address");
        entity.Property(e => e.IdCountry).HasColumnName("Id_Country");
        entity.Property(e => e.OrganisationId).HasColumnName("Organisation_Id");
    }
}

public class WorksheetCommentConfiguration : IEntityTypeConfiguration<WorksheetComment>
{
    public void Configure(EntityTypeBuilder<WorksheetComment> entity)
    {
        entity.HasNoKey().ToView("WorksheetComments");

        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.WorksheetId).HasColumnName("Id_Worksheet");
    }
}

public class WorksheetItemReportingConfiguration : IEntityTypeConfiguration<WorksheetItemReporting>
{
    public void Configure(EntityTypeBuilder<WorksheetItemReporting> entity)
    {
        entity.HasNoKey().ToView("WorksheetItemReporting");

        entity.Property(e => e.WorksheetItemId).HasColumnName("WorksheetItem_Id");
        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");
        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
    }
}

public class WorksheetItemsStatusReportingConfiguration : IEntityTypeConfiguration<WorksheetItemsStatusReporting>
{
    public void Configure(EntityTypeBuilder<WorksheetItemsStatusReporting> entity)
    {
        entity.HasNoKey().ToView("WorksheetItemsStatusReporting");

        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
    }
}

public class WorksheetReportingConfiguration : IEntityTypeConfiguration<WorksheetReporting>
{
    public void Configure(EntityTypeBuilder<WorksheetReporting> entity)
    {
        entity.HasNoKey().ToView("WorksheetReporting");

        entity.Property(e => e.WorksheetId).HasColumnName("Worksheet_Id");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
    }
}
