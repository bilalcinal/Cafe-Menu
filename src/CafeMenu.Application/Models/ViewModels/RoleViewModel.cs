using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class RoleViewModel
{
    public int RoleId { get; set; }

    [Required(ErrorMessage = "Rol adı gereklidir")]
    [StringLength(100, ErrorMessage = "Rol adı en fazla 100 karakter olabilir")]
    public string Name { get; set; } = string.Empty;

    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public List<int> SelectedPermissionIds { get; set; } = new();
    public List<PermissionDto> AvailablePermissions { get; set; } = new();
    public List<TenantDto> AvailableTenants { get; set; } = new();
}

