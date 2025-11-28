using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class TenantViewModel
{
    public int TenantId { get; set; }

    [Required(ErrorMessage = "Tenant adı gereklidir")]
    [StringLength(200, ErrorMessage = "Tenant adı en fazla 200 karakter olabilir")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tenant kodu gereklidir")]
    [StringLength(50, ErrorMessage = "Tenant kodu en fazla 50 karakter olabilir")]
    [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Tenant kodu sadece büyük harf, rakam, tire ve alt çizgi içerebilir")]
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

