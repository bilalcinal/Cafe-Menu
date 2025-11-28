using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("CATEGORY");

        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
            .HasColumnName("CATEGORYID");

        builder.Property(c => c.CategoryName)
            .HasColumnName("CATEGORYNAME")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.ParentCategoryId)
            .HasColumnName("PARENTCATEGORYID");

        builder.Property(c => c.IsDeleted)
            .HasColumnName("ISDELETED")
            .IsRequired();

        builder.Property(c => c.CreatedDate)
            .HasColumnName("CREATEDDATE")
            .IsRequired();

        builder.Property(c => c.CreatorUserId)
            .HasColumnName("CREATORUSERID");

        builder.Property(c => c.TenantId)
            .HasColumnName("TENANTID")
            .IsRequired();

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.TenantId);
    }
}

