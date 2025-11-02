using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task SetAvailabilityAsync(int productId, bool isAvailable);
        Task<IEnumerable<Product>> GetAvailableAsync();

       
        Task DecreaseStockAsync(int productId, int quantity);
    }
}
