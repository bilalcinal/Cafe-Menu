namespace CafeMenu.Application.Models.ViewModels;

public class MenuCategoryViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<MenuCategoryViewModel> SubCategories { get; set; } = new();
    public List<MenuProductViewModel> Products { get; set; } = new();
    public bool IsParent => SubCategories.Any();
}

