using CafeMenu.Application.Models;

namespace CafeMenu.Application.Interfaces.Services;

public interface ICurrencyService
{
    Task<CurrencyRatesDto> GetLatestRatesAsync(CancellationToken cancellationToken = default);
}

