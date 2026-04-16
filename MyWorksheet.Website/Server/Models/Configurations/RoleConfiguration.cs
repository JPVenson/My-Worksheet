using MyWorksheet.Webpage.Helper.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.HasKey(e => e.RoleId).HasName("PK__Role__D80AB4BB68E9D82A");

        entity.ToTable("Role");

        entity.Property(e => e.RoleId).HasColumnName("Role_Id");
        entity.Property(e => e.RoleName)
            .IsRequired()
            .HasMaxLength(250);

        foreach (var role in Roles.Yield())
        {
            entity.HasData(new Role()
            {
                RoleId = role.Id,
                RoleName = role.Name
            });
        }
    }
}
