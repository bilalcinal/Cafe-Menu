using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
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

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var properties = await _propertyService.GetAllAsync(cancellationToken);
        return View(properties);
    }

    [HttpGet]
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
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _propertyService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

