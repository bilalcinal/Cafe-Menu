using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("PRODUCT");

        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.ProductId)
            .HasColumnName("PRODUCTID");

        builder.Property(p => p.ProductName)
            .HasColumnName("PRODUCNAME")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .HasColumnName("CATEGORYID")
            .IsRequired();

        builder.Property(p => p.Price)
            .HasColumnName("PRICE")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.ImagePath)
            .HasColumnName("IMAGEPATH")
            .HasMaxLength(500);

        builder.Property(p => p.IsDeleted)
            .HasColumnName("ISDELETED")
            .IsRequired();

        builder.Property(p => p.CreatedDate)
            .HasColumnName("CREATEDDATE")
            .IsRequired();

        builder.Property(p => p.CreatorUserId)
            .HasColumnName("CREATORUSERID");

        builder.Property(p => p.TenantId)
            .HasColumnName("TENANTID")
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.CategoryId);
    }
}

