using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("ROLEPERMISSION");

        builder.HasKey(rp => rp.RolePermissionId);

        builder.Property(rp => rp.RolePermissionId)
            .HasColumnName("ROLEPERMISSIONID");

        builder.Property(rp => rp.RoleId)
            .HasColumnName("ROLEID")
            .IsRequired();

        builder.Property(rp => rp.PermissionId)
            .HasColumnName("PERMISSIONID")
            .IsRequired();

        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();
    }
}

