using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class UserViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Ad gereklidir")]
    [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad gereklidir")]
    [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [StringLength(100, ErrorMessage = "Kullanıcı adı en fazla 100 karakter olabilir")]
    public string UserName { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Şifre en fazla 200 karakter olabilir")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Tenant seçimi gereklidir")]
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Rol seçimi gereklidir")]
    public int RoleId { get; set; }

    public List<TenantDto> AvailableTenants { get; set; } = new();
    public List<RoleDto> AvailableRoles { get; set; } = new();
}

