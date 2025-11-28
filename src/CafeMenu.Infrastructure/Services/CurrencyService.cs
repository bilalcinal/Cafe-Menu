using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CafeMenu.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IMemoryCache _cache;
    private const string CacheKey = "currency_rates";
    private const int CacheExpirationMinutes = 5;

    public CurrencyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<CurrencyRatesDto> GetLatestRatesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out CurrencyRatesDto? cachedRates) && cachedRates != null)
        {
            return cachedRates;
        }

        await Task.CompletedTask;

        var rates = new CurrencyRatesDto
        {
            BaseCurrency = "TRY",
            TryRate = 1.0m,
            UsdRate = 34.50m,
            EurRate = 37.25m,
            LastUpdated = DateTime.UtcNow
        };

        _cache.Set(CacheKey, rates, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return rates;
    }

}

