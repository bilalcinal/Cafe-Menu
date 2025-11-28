using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("ROLE");

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
            .HasColumnName("ROLEID");

        builder.Property(r => r.Name)
            .HasColumnName("NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.IsSystem)
            .HasColumnName("ISSYSTEM")
            .IsRequired();

        builder.Property(r => r.IsActive)
            .HasColumnName("ISACTIVE")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.TenantId)
            .HasColumnName("TENANTID")
            .IsRequired();

        builder.Property(r => r.CreatedDate)
            .HasColumnName("CREATEDDATE")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.CreatorUserId)
            .HasColumnName("CREATEDBYUSERID");

        builder.Property(r => r.IsDeleted)
            .HasColumnName("ISDELETED")
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(r => r.Tenant)
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.CreatedByUser)
            .WithMany()
            .HasForeignKey(r => r.CreatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.Name, r.TenantId })
            .IsUnique();
    }
}

