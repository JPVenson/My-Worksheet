using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class PromisedFeatureContentConfiguration : IEntityTypeConfiguration<PromisedFeatureContent>
{
    public void Configure(EntityTypeBuilder<PromisedFeatureContent> entity)
    {
        entity.HasKey(e => e.PromisedFeatureContentId).HasName("PK__tmp_ms_x__0B5A49DFCEF4D3B3");

        entity.ToTable("PromisedFeatureContent");

        entity.Property(e => e.PromisedFeatureContentId).HasColumnName("PromisedFeatureContent_Id");
        entity.Property(e => e.DescriptionLong).IsRequired();
        entity.Property(e => e.IdPromisedFeature).HasColumnName("Id_PromisedFeature");
        entity.Property(e => e.IdPromisedFeatureRegion).HasColumnName("Id_PromisedFeatureRegion");
        entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

        entity.HasOne(d => d.IdPromisedFeatureNavigation).WithMany(p => p.PromisedFeatureContents)
            .HasForeignKey(d => d.IdPromisedFeature)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PromisedFeatureContent_PromisedFeature");

        entity.HasOne(d => d.IdPromisedFeatureRegionNavigation).WithMany(p => p.PromisedFeatureContents)
            .HasForeignKey(d => d.IdPromisedFeatureRegion)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PromisedFeatureContent_Region");
    }
}
