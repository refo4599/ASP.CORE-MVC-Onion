using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;

        public AdminDashboardController(
            ICategoryService categoryService,
            IProductService productService,
            IOrderService orderService,
            UserManager<AppUser> userManager)
        {
            _categoryService = categoryService;
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: /AdminDashboard/Index
        public async Task<IActionResult> Index()
        {
            
            var categories = await _categoryService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            var orders = await _orderService.GetOrdersForAdminAsync(); 
            var usersCount = _userManager.Users.Count();

            ViewBag.TotalCategories = categories?.Count() ?? 0;
            ViewBag.TotalProducts = products?.Count() ?? 0;
            ViewBag.TotalOrders = orders?.Count() ?? 0;
            ViewBag.TotalUsers = usersCount;

           
            ViewBag.RecentOrders = orders?.OrderByDescending(o => o.CreatedAt).Take(6).ToList();

            return View("Dashboard");
        }
    }
}
