using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("PROPERTY");

        builder.HasKey(p => p.PropertyId);

        builder.Property(p => p.PropertyId)
            .HasColumnName("PROPERTYID");

        builder.Property(p => p.Key)
            .HasColumnName("KEY")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Value)
            .HasColumnName("VALUE")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.TenantId)
            .HasColumnName("TENANTID")
            .IsRequired();

        builder.HasIndex(p => p.TenantId);
    }
}

