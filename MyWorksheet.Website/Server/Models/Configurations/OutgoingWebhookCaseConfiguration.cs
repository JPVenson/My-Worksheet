using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class OutgoingWebhookCaseConfiguration : IEntityTypeConfiguration<OutgoingWebhookCase>
{
    public void Configure(EntityTypeBuilder<OutgoingWebhookCase> entity)
    {
        entity.HasKey(e => e.OutgoingWebhookCaseId).HasName("PK__Outgoing__34D0A6874D990802");

        entity.ToTable("OutgoingWebhookCase");

        entity.Property(e => e.OutgoingWebhookCaseId).HasColumnName("OutgoingWebhookCase_Id");
        entity.Property(e => e.DescriptionHtml).IsRequired();
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(250);

        entity.HasData(new OutgoingWebhookCase()
        {
            Name = "Worksheet",
            DescriptionHtml = "Worksheet has changed",
            OutgoingWebhookCaseId = new Guid("00000000-0000-0000-0006-000000000001")
        });
        entity.HasData(new OutgoingWebhookCase()
        {
            Name = "Project",
            DescriptionHtml = "Project changed",
            OutgoingWebhookCaseId = new Guid("00000000-0000-0000-0006-000000000002")
        });
        entity.HasData(new OutgoingWebhookCase()
        {
            Name = "Worksheet Item",
            DescriptionHtml = "Worksheet Item changed",
            OutgoingWebhookCaseId = new Guid("00000000-0000-0000-0006-000000000003")
        });
    }
}
