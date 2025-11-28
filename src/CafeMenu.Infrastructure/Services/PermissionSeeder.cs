using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Services;

public static class PermissionSeeder
{
    public static async Task SeedPermissionsAsync(CafeMenuDbContext context)
    {
        var permissions = new List<Permission>
        {
            new Permission { Key = "Admin.Dashboard.View", DisplayName = "Dashboard Görüntüleme", GroupName = "Dashboard", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Category.List", DisplayName = "Kategori Listeleme", GroupName = "Kategori Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Category.Create", DisplayName = "Kategori Oluşturma", GroupName = "Kategori Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Category.Edit", DisplayName = "Kategori Düzenleme", GroupName = "Kategori Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Category.Delete", DisplayName = "Kategori Silme", GroupName = "Kategori Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Product.List", DisplayName = "Ürün Listeleme", GroupName = "Ürün Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Product.Create", DisplayName = "Ürün Oluşturma", GroupName = "Ürün Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Product.Edit", DisplayName = "Ürün Düzenleme", GroupName = "Ürün Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Product.Delete", DisplayName = "Ürün Silme", GroupName = "Ürün Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Property.List", DisplayName = "Özellik Listeleme", GroupName = "Özellik Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Property.Create", DisplayName = "Özellik Oluşturma", GroupName = "Özellik Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Property.Edit", DisplayName = "Özellik Düzenleme", GroupName = "Özellik Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Property.Delete", DisplayName = "Özellik Silme", GroupName = "Özellik Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Tenant.List", DisplayName = "Tenant Listeleme", GroupName = "Tenant Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Tenant.Create", DisplayName = "Tenant Oluşturma", GroupName = "Tenant Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Tenant.Edit", DisplayName = "Tenant Düzenleme", GroupName = "Tenant Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Tenant.Delete", DisplayName = "Tenant Silme", GroupName = "Tenant Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.User.List", DisplayName = "Kullanıcı Listeleme", GroupName = "Kullanıcı Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.User.Create", DisplayName = "Kullanıcı Oluşturma", GroupName = "Kullanıcı Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.User.Edit", DisplayName = "Kullanıcı Düzenleme", GroupName = "Kullanıcı Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.User.Delete", DisplayName = "Kullanıcı Silme", GroupName = "Kullanıcı Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Role.List", DisplayName = "Rol Listeleme", GroupName = "Rol Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Role.Create", DisplayName = "Rol Oluşturma", GroupName = "Rol Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Role.Edit", DisplayName = "Rol Düzenleme", GroupName = "Rol Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Role.Delete", DisplayName = "Rol Silme", GroupName = "Rol Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Permission { Key = "Admin.Role.ManagePermissions", DisplayName = "Rol İzinlerini Yönetme", GroupName = "Rol Yönetimi", CreatedDate = DateTime.UtcNow, IsActive = true }
        };

        foreach (var permission in permissions)
        {
            var existing = await context.Permissions
                .FirstOrDefaultAsync(p => p.Key == permission.Key);

            if (existing == null)
            {
                await context.Permissions.AddAsync(permission);
            }
            else if (existing.CreatedDate == default || existing.CreatedDate == DateTime.MinValue)
            {
                existing.CreatedDate = DateTime.UtcNow;
                context.Permissions.Update(existing);
            }
        }

        await context.SaveChangesAsync();
    }
}

