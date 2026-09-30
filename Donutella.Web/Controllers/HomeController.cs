using Donutella.Application.Interfaces;
using Donutella.Application.Services;
using Donutella.Web.Models;
using Donutella.Web.Models.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Donutella.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();

            var models = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image,
                SubCategoryId = p.SubCategoryId,
                SubCategoryName = p.SubCategory?.Name ?? string.Empty
            });

            return View(models);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
