using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Application.Services;

public class PropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ITenantResolver _tenantResolver;

    public PropertyService(
        IPropertyRepository propertyRepository,
        ITenantResolver tenantResolver)
    {
        _propertyRepository = propertyRepository;
        _tenantResolver = tenantResolver;
    }

    public async Task<IReadOnlyList<PropertyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var properties = await _propertyRepository.GetAllForTenantAsync(tenantId, cancellationToken);

        return properties
            .Select(p => new PropertyDto
            {
                PropertyId = p.PropertyId,
                Key = p.Key,
                Value = p.Value
            })
            .ToList();
    }

    public async Task<PropertyViewModel?> GetByIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var property = await _propertyRepository
            .Query()
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.TenantId == tenantId, cancellationToken);

        if (property == null)
            return null;

        return new PropertyViewModel
        {
            PropertyId = property.PropertyId,
            Key = property.Key,
            Value = property.Value
        };
    }

    public async Task<int> CreateAsync(PropertyViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();

        var property = new Property
        {
            Key = viewModel.Key,
            Value = viewModel.Value,
            TenantId = tenantId
        };

        await _propertyRepository.AddAsync(property, cancellationToken);
        await _propertyRepository.SaveChangesAsync(cancellationToken);
        return property.PropertyId;
    }

    public async Task UpdateAsync(PropertyViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var property = await _propertyRepository
            .Query()
            .FirstOrDefaultAsync(p => p.PropertyId == viewModel.PropertyId && p.TenantId == tenantId, cancellationToken);

        if (property == null)
            throw new InvalidOperationException("Property bulunamadı");

        property.Key = viewModel.Key;
        property.Value = viewModel.Value;

        _propertyRepository.Update(property);
        await _propertyRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var property = await _propertyRepository
            .Query()
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.TenantId == tenantId, cancellationToken);
        
        if (property != null)
        {
            _propertyRepository.Remove(property);
            await _propertyRepository.SaveChangesAsync(cancellationToken);
        }
    }
}

