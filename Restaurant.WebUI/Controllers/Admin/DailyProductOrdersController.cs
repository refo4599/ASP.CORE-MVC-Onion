using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDailyProductOrdersController : Controller
    {
        private readonly IDailyProductOrderService _dailyService;

        public AdminDailyProductOrdersController(IDailyProductOrderService dailyService)
        {
            _dailyService = dailyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTodayOrders()
        {
            var data = await _dailyService.GetTodayOrdersAsync();
            return Json(data);
        }
    }
}
