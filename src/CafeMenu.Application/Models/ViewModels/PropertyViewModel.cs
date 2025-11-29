using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class PropertyViewModel
{
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Özellik anahtarı gereklidir")]
    [StringLength(100, ErrorMessage = "Özellik anahtarı en fazla 100 karakter olabilir")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Özellik değeri gereklidir")]
    [StringLength(200, ErrorMessage = "Özellik değeri en fazla 200 karakter olabilir")]
    public string Value { get; set; } = string.Empty;
}

