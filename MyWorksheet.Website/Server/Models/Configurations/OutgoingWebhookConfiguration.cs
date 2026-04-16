using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class OutgoingWebhookConfiguration : IEntityTypeConfiguration<OutgoingWebhook>
{
    public void Configure(EntityTypeBuilder<OutgoingWebhook> entity)
    {
        entity.HasKey(e => e.OutgoingWebhookId).HasName("PK__tmp_ms_x__C4C1E8C55C66A95A");

        entity.ToTable("OutgoingWebhook");

        entity.Property(e => e.OutgoingWebhookId).HasColumnName("OutgoingWebhook_Id");
        entity.Property(e => e.CallingUrl)
            .IsRequired()
            .HasMaxLength(2000);
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdOutgoingWebhookCase).HasColumnName("Id_OutgoingWebhookCase");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.IsDeactivated).HasDefaultValue(true);
        entity.Property(e => e.NumberRangeEntry).HasMaxLength(400);
        entity.Property(e => e.Secret).IsRequired();

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.OutgoingWebhooks)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OutgoingWebhook_AppUser");

        entity.HasOne(d => d.IdOutgoingWebhookCaseNavigation).WithMany(p => p.OutgoingWebhooks)
            .HasForeignKey(d => d.IdOutgoingWebhookCase)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OutgoingWebhook_OutgoingWebhookCase");
    }
}
