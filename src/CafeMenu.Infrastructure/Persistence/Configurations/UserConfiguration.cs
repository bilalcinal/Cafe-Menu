using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeMenu.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USER");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("USERID");

        builder.Property(u => u.Name)
            .HasColumnName("NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Surname)
            .HasColumnName("SURNAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasColumnName("USERNAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.HashPassword)
            .HasColumnName("HASHPASSWORD")
            .HasColumnType("varbinary(64)")
            .IsRequired();

        builder.Property(u => u.SaltPassword)
            .HasColumnName("SALTPASSWORD")
            .HasColumnType("varbinary(32)")
            .IsRequired();

        builder.Property(u => u.TenantId)
            .HasColumnName("TENANTID")
            .IsRequired();

        builder.HasIndex(u => new { u.UserName, u.TenantId })
            .IsUnique();
    }
}

