using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class StorageTypeConfiguration : IEntityTypeConfiguration<StorageType>
{
    public void Configure(EntityTypeBuilder<StorageType> entity)
    {
        entity.HasKey(e => e.StorageTypeId).HasName("PK__StorageT__087172C4EA898CBA");

        entity.ToTable("StorageType");

        entity.Property(e => e.StorageTypeId).HasColumnName("StorageType_Id");

        entity.HasData(new StorageType()
        {
            Name = "Report",
            StorageTypeId = new Guid("00000000-0000-0000-0002-000000000001")
        }, new StorageType()
        {
            Name = "WorksheetAssert",
            StorageTypeId = new Guid("00000000-0000-0000-0002-000000000002")
        }, new StorageType()
        {
            Name = "Test",
            StorageTypeId = new Guid("00000000-0000-0000-0002-000000000003")
        }, new StorageType()
        {
            Name = "WorksheetDocument",
            StorageTypeId = new Guid("00000000-0000-0000-0002-000000000004")
        }, new StorageType()
        {
            Name = "Thumbnail",
            StorageTypeId = new Guid("00000000-0000-0000-0002-000000000005")
        }, new StorageType()
        {
            Name = "Email",
            StorageTypeId = new Guid("00000000-0000-0000-0002-000000000006")
        });
    }
}
