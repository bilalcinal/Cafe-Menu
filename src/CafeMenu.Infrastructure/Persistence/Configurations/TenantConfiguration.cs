using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("TENANT");

        builder.HasKey(t => t.TenantId);

        builder.Property(t => t.TenantId)
            .HasColumnName("TENANTID");

        builder.Property(t => t.Name)
            .HasColumnName("NAME")
            .HasMaxLength(200)
            .IsRequired();
    }
}

