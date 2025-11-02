using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<Category>> GetActiveCategoriesWithProductsAsync();
        Task<List<Category>> GetAllWithProductsAsync();
    }
}
