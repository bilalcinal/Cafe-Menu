using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace CafeMenu.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private const string CacheKey = "currency_rates";
    private const int CacheExpirationSeconds = 8;
    private const string CurrencyApiUrl = "http://any.kur.com/kurlar/web/rest";

    public CurrencyService(IMemoryCache cache, HttpClient httpClient)
    {
        _cache = cache;
        _httpClient = httpClient;
    }

    public async Task<CurrencyRatesDto> GetLatestRatesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out CurrencyRatesDto? cachedRates) && cachedRates != null)
            return cachedRates;

        try
        {
            var response = await _httpClient.GetAsync(CurrencyApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDoc = JsonDocument.Parse(jsonContent);
            var root = jsonDoc.RootElement;

            decimal? usdRate = null;
            decimal? eurRate = null;

            if (root.TryGetProperty("usd", out var usdElement) && usdElement.ValueKind == JsonValueKind.Number)
                usdRate = usdElement.GetDecimal();
            else if (root.TryGetProperty("USD", out var usdElement2) && usdElement2.ValueKind == JsonValueKind.Number)
                usdRate = usdElement2.GetDecimal();
            else if (root.TryGetProperty("usdTry", out var usdTryElement) && usdTryElement.ValueKind == JsonValueKind.Number)
                usdRate = usdTryElement.GetDecimal();

            if (root.TryGetProperty("eur", out var eurElement) && eurElement.ValueKind == JsonValueKind.Number)
                eurRate = eurElement.GetDecimal();
            else if (root.TryGetProperty("EUR", out var eurElement2) && eurElement2.ValueKind == JsonValueKind.Number)
                eurRate = eurElement2.GetDecimal();
            else if (root.TryGetProperty("eurTry", out var eurTryElement) && eurTryElement.ValueKind == JsonValueKind.Number)
                eurRate = eurTryElement.GetDecimal();

            if (usdRate is null || eurRate is null)
                throw new InvalidOperationException("Kur verisi beklenen formatta değil.");

            var rates = new CurrencyRatesDto
            {
                BaseCurrency = "TRY",
                TryRate = 1.0m,
                UsdRate = usdRate.Value,
                EurRate = eurRate.Value,
                LastUpdated = DateTime.UtcNow
            };

            _cache.Set(CacheKey, rates, TimeSpan.FromSeconds(CacheExpirationSeconds));

            return rates;
        }
        catch
        {
            var fallbackRates = new CurrencyRatesDto
            {
                BaseCurrency = "TRY",
                TryRate = 1.0m,
                UsdRate = 40m,
                EurRate = 45m,
                LastUpdated = DateTime.UtcNow
            };

            _cache.Set(CacheKey, fallbackRates, TimeSpan.FromSeconds(30));
            return fallbackRates;
        }
    }
}
