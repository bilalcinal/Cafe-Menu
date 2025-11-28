using CafeMenu.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Controllers;

public class CustomerController : Controller
{
    private readonly CustomerMenuService _customerMenuService;

    public CustomerController(CustomerMenuService customerMenuService)
    {
        _customerMenuService = customerMenuService;
    }

    public async Task<IActionResult> Index(int? categoryId, CancellationToken cancellationToken = default)
    {
        var viewModel = await _customerMenuService.GetMenuAsync(categoryId, cancellationToken);
        return View(viewModel);
    }
}

