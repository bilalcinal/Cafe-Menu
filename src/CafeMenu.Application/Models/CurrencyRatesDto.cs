namespace CafeMenu.Application.Models;

public class CurrencyRatesDto
{
    public string BaseCurrency { get; set; } = "TRY";
    public decimal TryRate { get; set; } = 1.0m;
    public decimal UsdRate { get; set; } = 1.0m;
    public decimal EurRate { get; set; } = 1.0m;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public decimal Usd => UsdRate;
    public decimal Eur => EurRate;
    public decimal Try => TryRate;
    public DateTime FetchedAt => LastUpdated;
}

