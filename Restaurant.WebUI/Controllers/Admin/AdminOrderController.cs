using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

     
        public async Task<IActionResult> Index(string? status)
        {
            List<Order> orders;
            if (!string.IsNullOrEmpty(status))
            {
                orders = await _orderService.GetOrdersByStatusAsync(status);
                ViewBag.SelectedStatus = status;
            }
            else
            {
                orders = await _orderService.GetOrdersForAdminAsync();
            }

            return View(orders);
        }

    
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = " Order not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(order); // Order + Items 
        }

        // AJAX
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return Json(new { success = false, message = "Order not found!" });

            if (order.Status == "Cancelled" || order.Status == "Completed")
                return Json(new { success = false, message = "Cannot change status of completed/cancelled order!" });

            if (status == "Cancelled")
            {
                
                await _orderService.DeleteAsync(id);
                return Json(new { success = true, deleted = true });
            }
            else
            {
                await _orderService.UpdateStatusAsync(id, status);
                return Json(new { success = true, newStatus = status });
            }
        }

    }
}
