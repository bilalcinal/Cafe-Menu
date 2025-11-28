namespace CafeMenu.Application.Models.ViewModels;

public class DashboardViewModel
{
    public Dictionary<string, int> ProductCountByCategory { get; set; } = new();
    public CurrencyRatesDto CurrencyRates { get; set; } = new();
}

