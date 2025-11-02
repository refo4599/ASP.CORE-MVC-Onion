using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task AddCartAsync(Cart cart);
        Task SaveChangesAsync();
        Task<List<CartItem>> GetCartItemsAsync(string userId); 
        Task ClearCartAsync(string userId);                  
    }
}
