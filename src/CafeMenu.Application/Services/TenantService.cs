using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Services;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return tenants.Select(t => new TenantDto
        {
            TenantId = t.TenantId,
            Name = t.Name,
            Code = t.Code,
            IsActive = t.IsActive,
            CreatedDate = t.CreatedDate
        }).ToList();
    }

    public async Task<TenantViewModel?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
            return null;

        return new TenantViewModel
        {
            TenantId = tenant.TenantId,
            Name = tenant.Name,
            Code = tenant.Code,
            IsActive = tenant.IsActive
        };
    }

    public async Task<int> CreateAsync(TenantViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var existing = await _tenantRepository.GetByCodeAsync(viewModel.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("Bu kod zaten kullanılıyor");

        var tenant = new Tenant
        {
            Name = viewModel.Name,
            Code = viewModel.Code.ToUpperInvariant(),
            IsActive = viewModel.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _tenantRepository.AddAsync(tenant, cancellationToken);
        return created.TenantId;
    }

    public async Task UpdateAsync(TenantViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(viewModel.TenantId, cancellationToken);
        if (tenant == null)
            throw new InvalidOperationException("Tenant bulunamadı");

        var existing = await _tenantRepository.GetByCodeAsync(viewModel.Code, cancellationToken);
        if (existing != null && existing.TenantId != viewModel.TenantId)
            throw new InvalidOperationException("Bu kod zaten kullanılıyor");

        tenant.Name = viewModel.Name;
        tenant.Code = viewModel.Code.ToUpperInvariant();
        tenant.IsActive = viewModel.IsActive;

        await _tenantRepository.UpdateAsync(tenant);
    }

    public async Task DeleteAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        await _tenantRepository.DeleteAsync(tenantId, cancellationToken);
    }
}

