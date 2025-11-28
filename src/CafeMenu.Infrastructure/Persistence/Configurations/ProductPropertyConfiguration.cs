using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class ProductPropertyConfiguration : IEntityTypeConfiguration<ProductProperty>
{
    public void Configure(EntityTypeBuilder<ProductProperty> builder)
    {
        builder.ToTable("PRODUCTPROPERTY");

        builder.HasKey(pp => pp.ProductPropertyId);

        builder.Property(pp => pp.ProductPropertyId)
            .HasColumnName("PRODUCTPROPERTYID");

        builder.Property(pp => pp.ProductId)
            .HasColumnName("PRODUCTID")
            .IsRequired();

        builder.Property(pp => pp.PropertyId)
            .HasColumnName("PROPERTYID")
            .IsRequired();

        builder.HasOne(pp => pp.Product)
            .WithMany(p => p.ProductProperties)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Property)
            .WithMany(p => p.ProductProperties)
            .HasForeignKey(pp => pp.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pp => pp.ProductId);
    }
}

