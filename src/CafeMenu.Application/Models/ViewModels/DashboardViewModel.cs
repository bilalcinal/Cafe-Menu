namespace CafeMenu.Application.Models.ViewModels;

public class DashboardViewModel
{
    public Dictionary<string, int> ProductCountByCategory { get; set; } = new();
    public CurrencyRatesDto CurrencyRates { get; set; } = new();

    public List<CategoryProductCountDto> CategoryProductCounts => ProductCountByCategory
        .OrderByDescending(x => x.Value)
        .Select(kvp => new CategoryProductCountDto
        {
            CategoryName = kvp.Key,
            ProductCount = kvp.Value
        })
        .ToList();
}

public class CategoryProductCountDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}

