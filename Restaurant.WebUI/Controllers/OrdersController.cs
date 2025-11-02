using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly UserManager<AppUser> _userManager;

        public UserOrderController(
            IOrderService orderService,
            ICartService cartService,
            IProductService productService,
            UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["Error"] = " Cart is empty!";
                return RedirectToAction("Index", "Menu");
            }
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string orderType, string? deliveryAddress, string? phoneNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartItemsAsync(userId);

            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            if (orderType == "Delivery")
            {
                if (string.IsNullOrWhiteSpace(deliveryAddress) || string.IsNullOrWhiteSpace(phoneNumber))
                {
                    TempData["Error"] = " Address and phone number are required for delivery orders..";
                    return RedirectToAction("Checkout");
                }
            }

            await _orderService.CreateOrderAsync(userId, cartItems, orderType, deliveryAddress, phoneNumber);
            await _cartService.ClearCartAsync(userId);

            TempData["Message"] = $" Order placed successfully as '{orderType}'!";
            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            foreach (var order in orders)
            {
                decimal originalSubtotal = 0;

                foreach (var item in order.Items)
                {
                    originalSubtotal += item.Product.Price * item.Quantity;

                    item.Price = _productService.GetFinalPrice(item.Product);
                }

                order.Subtotal = order.Items.Sum(i => i.Price * i.Quantity);

                order.Discount = originalSubtotal - order.Subtotal;

                order.TotalPrice = order.Subtotal + order.TaxAmount - order.Discount;
            }

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        public async Task<IActionResult> Track(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null || order.UserId != user.Id)
                return NotFound();

            order.Subtotal = order.Items.Sum(i => i.Price * i.Quantity);
            order.Discount = order.Items.Sum(i => (i.Product.Price - i.Price) * i.Quantity);
            order.TaxAmount = Math.Round(order.Subtotal * 0.085m, 2);
            order.TotalPrice = order.Subtotal + order.TaxAmount - order.Discount;

            if (order.Status == "Completed")
                ViewBag.Notification = "تم اكتمال طلبك بنجاح! شكراً لتعاملك معنا 💚";
            else if (order.Status == "Ready")
            {
                ViewBag.Notification = "طلبك جاهز للتوصيل!";

                if (order.ReadyAt.HasValue)
                {
                    // Delivery Time 

                    var elapsed = DateTime.Now - order.ReadyAt.Value;
                    var remaining = TimeSpan.FromMinutes(3) - elapsed;
                    if (remaining.TotalMinutes > 0)
                        ViewBag.RemainingMinutes = (int)Math.Ceiling(remaining.TotalMinutes);
                    else
                        ViewBag.RemainingMinutes = 0;
                }
            }
            else if (order.Status == "Preparing")
                ViewBag.Notification = "طلبك قيد التحضير...";
            else
                ViewBag.Notification = "تم استلام طلبك، برجاء المتابعة.";

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = " الطلب غير موجود.";
                return RedirectToAction(nameof(MyOrders));
            }

            if (order.Status == "Ready" || order.Status == "Completed" || order.Status == "Cancelled")
            {
                TempData["Error"] = " لا يمكن إلغاء هذا الطلب بعد أن يكون جاهز أو مكتمل أو ملغى.";
                return RedirectToAction(nameof(Track), new { id = id });
            }

            await _orderService.DeleteAsync(id);
            TempData["Message"] = " تم إلغاء الطلب بنجاح!";
            return RedirectToAction(nameof(MyOrders));
        }
    }
}
