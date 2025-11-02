using Restaurant.Application.ViewModels;
using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<(bool Success, string Message, int RemainingStock)> AddToCartAsync(string userId, CartItemVM product);
        Task UpdateQuantityAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetTotalAsync(string userId);

       
        Task<List<OrderItems>> GetCartItemsAsync(string userId);
    }
}
