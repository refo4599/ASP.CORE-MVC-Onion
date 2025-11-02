using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task<List<Order>> GetOrdersWithItemsAsync();
        Task UpdateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByStatusAsync(string status);
        Task<int> CountOrdersForProductTodayAsync(int productId);
        Task UpdateStatusAsync(int orderId, string newStatus);
        Task RequestFeedbackAsync(int orderId);


    }
}
