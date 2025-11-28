namespace CafeMenu.Application.Models.ViewModels;

public class CustomerMenuViewModel
{
    public int TenantId { get; set; }
    public IReadOnlyList<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public int? SelectedCategoryId { get; set; }
    public IReadOnlyList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public CurrencyRatesDto CurrencyRates { get; set; } = new();
}

