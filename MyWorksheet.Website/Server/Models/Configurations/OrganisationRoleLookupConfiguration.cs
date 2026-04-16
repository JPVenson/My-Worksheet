using MyWorksheet.Webpage.Helper.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class OrganisationRoleLookupConfiguration : IEntityTypeConfiguration<OrganisationRoleLookup>
{
    public void Configure(EntityTypeBuilder<OrganisationRoleLookup> entity)
    {
        entity.HasKey(e => e.OrganisationRoleLookupId).HasName("PK__Organisa__CCD48189B32782BF");

        entity.ToTable("OrganisationRoleLookup");

        entity.Property(e => e.OrganisationRoleLookupId).HasColumnName("OrganisationRoleLookup_Id");
        entity.Property(e => e.Name).IsRequired();

        foreach (var role in UserToOrgRoles.Yield())
        {
            entity.HasData(new OrganisationRoleLookup()
            {
                Name = role.Name,
                OrganisationRoleLookupId = role.Id
            });
        }
    }
}
