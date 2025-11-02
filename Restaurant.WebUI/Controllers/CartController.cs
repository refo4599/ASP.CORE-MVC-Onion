using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using Restaurant.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ICartService cartService, IProductService productService, UserManager<AppUser> userManager)
        {
            _cartService = cartService;
            _productService = productService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to view your cart.";
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            var cartVM = cart.Items.Select(i =>
            {
                var originalPrice = i.Product.Price;
                var finalPrice = _productService.GetFinalPrice(i.Product); 

                return new CartItemVM
                {
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = finalPrice,      
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl,
                    OriginalPrice = originalPrice 
                };
            }).ToList();

            ViewBag.Total = cartVM.Sum(x => x.Price * x.Quantity);
            return View(cartVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not logged in" });

            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
                return Json(new { success = false, message = "Product not found" });

           
            var finalPrice = _productService.GetFinalPrice(product);

            await _cartService.AddToCartAsync(user.Id, new CartItemVM
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = finalPrice,
                Quantity = 1,
                ImageUrl = product.ImageUrl
            });

            return Json(new { success = true, message = $"{product.Name} added to cart ✅", finalPrice });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });

            await _cartService.UpdateQuantityAsync(user.Id, productId, quantity);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });

            await _cartService.RemoveFromCartAsync(user.Id, productId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false });

            await _cartService.ClearCartAsync(user.Id);
            return Json(new { success = true });
        }
    }
}
