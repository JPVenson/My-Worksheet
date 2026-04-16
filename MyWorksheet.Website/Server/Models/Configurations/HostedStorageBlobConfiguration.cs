using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class HostedStorageBlobConfiguration : IEntityTypeConfiguration<HostedStorageBlob>
{
    public void Configure(EntityTypeBuilder<HostedStorageBlob> entity)
    {
        entity.HasKey(e => e.HostedStorageBlobId).HasName("PK__HostedSt__A8E7782454F63DCE");

        entity.ToTable("HostedStorageBlob");

        entity.Property(e => e.HostedStorageBlobId).HasColumnName("HostedStorageBlob_Id");
        entity.Property(e => e.Key).HasDefaultValueSql("md5(random()::text || clock_timestamp()::text)::uuid");
    }
}
