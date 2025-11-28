using System.ComponentModel.DataAnnotations;
using CafeMenu.Application.Models;

namespace CafeMenu.Application.Models.ViewModels;

public class ProductViewModel
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Ürün adı gereklidir")]
    [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori seçimi gereklidir")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Fiyat gereklidir")]
    [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0.01 ile 999999.99 arasında olmalıdır")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Resim yolu en fazla 500 karakter olabilir")]
    public string ImagePath { get; set; } = string.Empty;

    public List<CategoryDto> AvailableCategories { get; set; } = new();
    public List<int> SelectedPropertyIds { get; set; } = new();
    public List<PropertyDto> AvailableProperties { get; set; } = new();
}

