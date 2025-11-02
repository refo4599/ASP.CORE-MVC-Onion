using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, List<OrderItems> cartItems, string orderType, string? deliveryAddress, string? phoneNumber);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task DeleteAsync(int orderId);
        Task<List<Order>> GetOrdersByStatusAsync(string status);
        Task<List<Order>> GetOrdersForAdminAsync();


        Task UpdateStatusAsync(int orderId, string newStatus);
        Task RequestFeedbackAsync(int orderId);


    }
}
