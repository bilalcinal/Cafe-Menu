using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Persistence;

public class CafeMenuDbContext : DbContext
{
    public CafeMenuDbContext(DbContextOptions<CafeMenuDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<ProductProperty> ProductProperties { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CafeMenuDbContext).Assembly);
    }
}

