using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using Restaurant.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers
{
    public class MenuController : Controller
    {
        private readonly IProductService _productService;
        private readonly IFavouriteService _favService;
        private readonly ICartService _cartService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryService _categoryService;

        public MenuController(
            IProductService productService,
            IFavouriteService favService,
            ICartService cartService,
            UserManager<AppUser> userManager,
            ICategoryService categoryService)
        {
            _productService = productService;
            _favService = favService;
            _cartService = cartService;
            _userManager = userManager;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllWithProductsAsync();

            foreach (var category in categories)
            {
                category.Products = category.Products
                    .Where(p => p.IsAvailable && p.InStock > 0)
                    .ToList();

                foreach (var product in category.Products)
                {
                    // Happy hour discount logic
                    var now = DateTime.Now.TimeOfDay;
                    var happyHourStart = new TimeSpan(19, 0, 0);
                    var happyHourEnd = new TimeSpan(23, 0, 0);

                    if (now >= happyHourStart && now <= happyHourEnd)
                    {
                        product.DiscountPrice = Math.Round(product.Price * 0.8m, 2);
                    }
                    else if (product.Price >= 100)
                    {
                        product.DiscountPrice = Math.Round(product.Price * 0.9m, 2);
                    }
                    else
                    {
                        product.DiscountPrice = null;
                    }
                }
            }

            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var now = DateTime.Now.TimeOfDay;
            if (now >= new TimeSpan(0, 0, 0) && now < new TimeSpan(6, 0, 0))
            {
                TempData["Error"] = "❌ المطعم مغلق حاليًا. الرجاء المحاولة بعد الساعة 6 صباحًا.";
                return RedirectToAction("Index", "Menu");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not logged in" });

            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
                return Json(new { success = false, message = "Product not found" });

            
            decimal finalPrice = product.DiscountPrice ?? _productService.GetFinalPrice(product);

            await _cartService.AddToCartAsync(user.Id, new CartItemVM
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = finalPrice,
                Quantity = 1,
                ImageUrl = product.ImageUrl,
                DiscountPrice = product.DiscountPrice
            });

            string msg = (product.DiscountPrice.HasValue)
                ? $"{product.Name} added to cart with discount 🎉"
                : $"{product.Name} added to cart ✅";

            return Json(new { success = true, message = msg, finalPrice });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavourite(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool added;

            if (user != null)
            {
              
                var userId = user.Id;
                added = await _favService.ToggleFavouriteAsync(id, userId);
            }
            else
            {
               
                var sessionData = HttpContext.Session.GetString("Favourites");
                var favourites = string.IsNullOrEmpty(sessionData)
                    ? new List<int>()
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(sessionData);

                if (favourites.Contains(id))
                {
                    favourites.Remove(id);
                    added = false;
                }
                else
                {
                    favourites.Add(id);
                    added = true;
                }

                HttpContext.Session.SetString("Favourites", Newtonsoft.Json.JsonConvert.SerializeObject(favourites));
            }

            return Json(new
            {
                success = true,
                added,
                message = added ? "تمت الإضافة إلى المفضلة ❤️" : "تمت الإزالة من المفضلة 💔"
            });
        }
        [HttpGet]
        public async Task<IActionResult> Favourites()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Product> favourites;

            if (user != null)
            {
              
                favourites = await _favService.GetFavouritesAsync(user.Id);
            }
            else
            {
                
                var sessionData = HttpContext.Session.GetString("Favourites");
                var favIds = string.IsNullOrEmpty(sessionData)
                    ? new List<int>()
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(sessionData);

                // DbContext
                var allProducts = await _productService.GetAllAsync();
                favourites = allProducts
                    .Where(p => favIds.Contains(p.Id))
                    .ToList();

            }

            var model = favourites.Select(p => new ProductMenuVM
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                ImageUrl = p.ImageUrl,
                InStock = p.InStock
            }).ToList();

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> GetProductStock(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return Json(new { inStock = 0 });

            return Json(new { inStock = product.InStock });
        }
    }
}
