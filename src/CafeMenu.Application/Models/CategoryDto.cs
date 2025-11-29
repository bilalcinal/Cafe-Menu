namespace CafeMenu.Application.Models;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
    public bool IsParent => Children.Any();
}

