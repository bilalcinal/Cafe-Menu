using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ProductController : Controller
{
    private readonly ProductService _productService;
    private readonly CategoryService _categoryService;
    private readonly PropertyService _propertyService;

    public ProductController(
        ProductService productService,
        CategoryService categoryService,
        PropertyService propertyService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _propertyService = propertyService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        var viewModel = new ProductViewModel
        {
            AvailableCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList(),
            AvailableProperties = (await _propertyService.GetAllAsync(cancellationToken)).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AvailableCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
            viewModel.AvailableProperties = (await _propertyService.GetAllAsync(cancellationToken)).ToList();
            return View(viewModel);
        }

        await _productService.CreateAsync(viewModel, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var viewModel = await _productService.GetByIdAsync(id, cancellationToken);
        if (viewModel == null)
        {
            return NotFound();
        }

        viewModel.AvailableCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
        viewModel.AvailableProperties = (await _propertyService.GetAllAsync(cancellationToken)).ToList();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AvailableCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
            viewModel.AvailableProperties = (await _propertyService.GetAllAsync(cancellationToken)).ToList();
            return View(viewModel);
        }

        try
        {
            await _productService.UpdateAsync(viewModel, cancellationToken);
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
        await _productService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

