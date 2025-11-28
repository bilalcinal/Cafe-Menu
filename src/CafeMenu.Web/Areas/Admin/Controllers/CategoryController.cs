using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CategoryController : Controller
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [RequirePermission("Category.View")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return View(categories);
    }

    [HttpGet]
    [RequirePermission("Category.Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        var viewModel = new CategoryViewModel
        {
            AvailableParentCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AvailableParentCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
            return View(viewModel);
        }

        await _categoryService.CreateAsync(viewModel, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Category.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var viewModel = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (viewModel == null)
        {
            return NotFound();
        }

        viewModel.AvailableParentCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AvailableParentCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
            return View(viewModel);
        }

        try
        {
            await _categoryService.UpdateAsync(viewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Category.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

