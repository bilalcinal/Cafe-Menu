using CafeMenu.Application.Models;

namespace CafeMenu.Application.Models.ViewModels;

public class RolePermissionsViewModel
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public List<int> SelectedPermissionIds { get; set; } = new();
    public List<PermissionDto> AvailablePermissions { get; set; } = new();
}

