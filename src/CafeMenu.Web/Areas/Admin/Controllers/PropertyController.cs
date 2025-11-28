using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class PropertyController : Controller
{
    private readonly PropertyService _propertyService;

    public PropertyController(PropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [RequirePermission("Admin.Property.List")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var properties = await _propertyService.GetAllAsync(cancellationToken);
        return View(properties);
    }

    [HttpGet]
    [RequirePermission("Admin.Property.Create")]
    public IActionResult Create()
    {
        return View(new PropertyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PropertyViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        await _propertyService.CreateAsync(viewModel, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Admin.Property.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var viewModel = await _propertyService.GetByIdAsync(id, cancellationToken);
        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PropertyViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            await _propertyService.UpdateAsync(viewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Admin.Property.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _propertyService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

