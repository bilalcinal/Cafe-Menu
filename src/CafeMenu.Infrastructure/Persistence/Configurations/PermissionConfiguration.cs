using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("PERMISSION");

        builder.HasKey(p => p.PermissionId);

        builder.Property(p => p.PermissionId)
            .HasColumnName("PERMISSIONID");

        builder.Property(p => p.Key)
            .HasColumnName("KEY")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.DisplayName)
            .HasColumnName("DISPLAYNAME")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.GroupName)
            .HasColumnName("GROUPNAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.CreatedDate)
            .HasColumnName("CREATEDDATE")
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasColumnName("ISACTIVE")
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(p => p.Key)
            .IsUnique();
    }
}

