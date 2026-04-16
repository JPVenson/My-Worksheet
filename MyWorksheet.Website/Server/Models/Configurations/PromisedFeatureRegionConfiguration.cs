using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class PromisedFeatureRegionConfiguration : IEntityTypeConfiguration<PromisedFeatureRegion>
{
    public void Configure(EntityTypeBuilder<PromisedFeatureRegion> entity)
    {
        entity.HasKey(e => e.PromisedFeatureRegionId).HasName("PK__tmp_ms_x__60049507A4ECC70B");

        entity.ToTable("PromisedFeatureRegion");

        entity.Property(e => e.PromisedFeatureRegionId).HasColumnName("PromisedFeatureRegion_Id");
        entity.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3);
        entity.Property(e => e.RegionName).IsRequired();

        entity.HasData([
            new PromisedFeatureRegion()
            {
                Currency = "\u20ac",
                IsActive = true,
                RegionName = "Germany",
                RegionShortName = "de",
                PromisedFeatureRegionId = new Guid("00000000-0000-0000-0007-000000000001")
            },
            new PromisedFeatureRegion()
            {
                Currency = "$",
                IsActive = true,
                RegionName = "United States of America",
                RegionShortName = "us",
                PromisedFeatureRegionId = new Guid("00000000-0000-0000-0007-000000000002")
            }
        ]);
    }
}
