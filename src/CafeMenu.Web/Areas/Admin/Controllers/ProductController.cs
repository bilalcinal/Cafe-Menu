using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Web.Attributes;
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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(
        ProductService productService,
        CategoryService categoryService,
        PropertyService propertyService,
        IWebHostEnvironment webHostEnvironment)
    {
        _productService = productService;
        _categoryService = categoryService;
        _propertyService = propertyService;
        _webHostEnvironment = webHostEnvironment;
    }

    [RequirePermission("Product.View")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return View(products);
    }

    [HttpGet]
    [RequirePermission("Product.Create")]
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
    public async Task<IActionResult> Create(ProductViewModel viewModel, IFormFile? imageFile, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AvailableCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
            viewModel.AvailableProperties = (await _propertyService.GetAllAsync(cancellationToken)).ToList();
            return View(viewModel);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream, cancellationToken);
            }

            viewModel.ImagePath = $"/uploads/products/{uniqueFileName}";
        }

        await _productService.CreateAsync(viewModel, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Product.Edit")]
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
    public async Task<IActionResult> Edit(ProductViewModel viewModel, IFormFile? imageFile, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AvailableCategories = (await _categoryService.GetAllAsync(cancellationToken)).ToList();
            viewModel.AvailableProperties = (await _propertyService.GetAllAsync(cancellationToken)).ToList();
            return View(viewModel);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var existingProduct = await _productService.GetByIdAsync(viewModel.ProductId, cancellationToken);
            if (existingProduct != null && !string.IsNullOrEmpty(existingProduct.ImagePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream, cancellationToken);
            }

            viewModel.ImagePath = $"/uploads/products/{uniqueFileName}";
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
    [RequirePermission("Product.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

