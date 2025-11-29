using CafeMenu.Application.Models.ViewModels;

namespace CafeMenu.Application.Interfaces.Services;

public interface IMenuPdfService
{
    Task<byte[]> GenerateMenuPdfAsync(MenuViewModel menuData, CancellationToken cancellationToken = default);
}

