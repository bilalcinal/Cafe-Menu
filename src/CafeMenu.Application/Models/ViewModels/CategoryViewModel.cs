using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class CategoryViewModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Kategori adı gereklidir")]
    [StringLength(200, ErrorMessage = "Kategori adı en fazla 200 karakter olabilir")]
    public string CategoryName { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
    public List<CategoryDto> AvailableParentCategories { get; set; } = new();
}

