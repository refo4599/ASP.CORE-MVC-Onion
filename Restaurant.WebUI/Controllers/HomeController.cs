using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using Restaurant.Models;
using Restaurant.WebUI.Models;
using System.Diagnostics;

namespace Restaurant.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        public HomeController(ILogger<HomeController> logger , ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllWithProductsAsync();
            //// hapy Day Hom

            var happyHourStart = new TimeSpan(9, 0, 0);
            var happyHourEnd = new TimeSpan(12, 0, 0);
            var now = DateTime.Now.TimeOfDay;

            var happyHourProducts = new List<ProductMenuVM>();

            foreach (var category in categories)
            {
                foreach (var product in category.Products)
                {
                    if (now >= happyHourStart && now <= happyHourEnd)
                    {
                        happyHourProducts.Add(new ProductMenuVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            ImageUrl = product.ImageUrl,
                            IsFavourite = false,
                            Quantity = 1,
                            InStock = product.InStock,
                            IsAvailable = product.IsAvailable,
                            DiscountPrice = Math.Round(product.Price * 0.8m, 2),
                            DiscountStart = happyHourStart,
                            DiscountEnd = happyHourEnd
                        });
                    }
                }
            }

            ViewBag.IsHappyHour = now >= happyHourStart && now <= happyHourEnd;
            ViewBag.HappyHourStart = happyHourStart;
            ViewBag.HappyHourEnd = happyHourEnd;
            ViewBag.HappyHourProducts = happyHourProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
