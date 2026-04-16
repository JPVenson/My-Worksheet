using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class UserAssosiatedRoleLookupConfiguration : IEntityTypeConfiguration<UserAssosiatedRoleLookup>
{
    public void Configure(EntityTypeBuilder<UserAssosiatedRoleLookup> entity)
    {
        entity.HasKey(e => e.UserAssosiatedRoleLookupId).HasName("PK__UserAsso__1891B2C46E63CA18");

        entity.ToTable("UserAssosiatedRoleLookup");

        entity.Property(e => e.UserAssosiatedRoleLookupId).HasColumnName("UserAssosiatedRoleLookup_Id");
        entity.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        entity.HasData(new UserAssosiatedRoleLookup() { UserAssosiatedRoleLookupId = new Guid("00000000-0000-0000-0008-000000000001"), Code = "S", Description = "Self" });
        entity.HasData(new UserAssosiatedRoleLookup() { UserAssosiatedRoleLookupId = new Guid("00000000-0000-0000-0008-000000000002"), Code = "WH", Description = "WorksheetHolder" });
        entity.HasData(new UserAssosiatedRoleLookup() { UserAssosiatedRoleLookupId = new Guid("00000000-0000-0000-0008-000000000003"), Code = "SA", Description = "Support Admin" });
        entity.HasData(new UserAssosiatedRoleLookup() { UserAssosiatedRoleLookupId = new Guid("00000000-0000-0000-0008-000000000004"), Code = "AD", Description = "Administrator" });
    }
}
