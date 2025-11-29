namespace CafeMenu.Application.Models.ViewModels;

public class MenuProductViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public List<string> Badges { get; set; } = new();
}

