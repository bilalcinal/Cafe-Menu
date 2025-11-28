using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace CafeMenu.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "currency_rates";
    private const int CacheExpirationMinutes = 5;

    public CurrencyService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<CurrencyRatesDto> GetLatestRatesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out CurrencyRatesDto? cachedRates) && cachedRates != null)
        {
            return cachedRates;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<CurrencyApiResponse>("http://any.kur.com/kurlar", cancellationToken);

            var rates = new CurrencyRatesDto
            {
                BaseCurrency = "TRY",
                TryRate = 1.0m,
                UsdRate = response?.UsdRate ?? 1.0m,
                EurRate = response?.EurRate ?? 1.0m,
                LastUpdated = DateTime.UtcNow
            };

            _cache.Set(CacheKey, rates, TimeSpan.FromMinutes(CacheExpirationMinutes));

            return rates;
        }
        catch
        {
            var fallbackRates = new CurrencyRatesDto
            {
                BaseCurrency = "TRY",
                TryRate = 1.0m,
                UsdRate = 1.0m,
                EurRate = 1.0m,
                LastUpdated = DateTime.UtcNow
            };

            if (_cache.TryGetValue(CacheKey, out CurrencyRatesDto? lastKnownRates) && lastKnownRates != null)
            {
                return lastKnownRates;
            }

            return fallbackRates;
        }
    }

    private class CurrencyApiResponse
    {
        public decimal UsdRate { get; set; }
        public decimal EurRate { get; set; }
    }
}

