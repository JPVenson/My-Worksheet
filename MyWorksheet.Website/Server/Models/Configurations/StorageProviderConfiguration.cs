using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class StorageProviderConfiguration : IEntityTypeConfiguration<StorageProvider>
{
    public void Configure(EntityTypeBuilder<StorageProvider> entity)
    {
        entity.HasKey(e => e.StorageProviderId).HasName("PK__tmp_ms_x__64324A8A0F48E341");

        entity.ToTable("StorageProvider");

        entity.Property(e => e.StorageProviderId).HasColumnName("StorageProvider_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.StorageKey).IsRequired();

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.StorageProviders)
            .HasForeignKey(d => d.IdAppUser)
            .HasConstraintName("FK_StorageProvider_AppUser");

        entity.HasData(new StorageProvider()
        {
            Name = "My-Worksheet Hosted",
            StorageKey = "LocalProvider",
            IsDefaultProvider = false,
            IdAppUser = null,
            StorageProviderId = new Guid("00000000-0000-0000-0005-000000000001")
        });
    }
}
