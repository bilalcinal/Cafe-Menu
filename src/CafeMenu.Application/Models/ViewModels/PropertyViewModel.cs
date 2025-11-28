using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class PropertyViewModel
{
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Key gereklidir")]
    [StringLength(100, ErrorMessage = "Key en fazla 100 karakter olabilir")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Value gereklidir")]
    [StringLength(200, ErrorMessage = "Value en fazla 200 karakter olabilir")]
    public string Value { get; set; } = string.Empty;
}

