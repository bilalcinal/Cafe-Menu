namespace CafeMenu.Application.Models.ViewModels;

public class MenuViewModel
{
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string? TenantLogo { get; set; }
    public List<MenuCategoryViewModel> Categories { get; set; } = new();
}

