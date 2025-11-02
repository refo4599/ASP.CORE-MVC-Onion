using Restaurant.Application.ViewModels;
using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<List<CategoryWithProductsVM>> GetAllWithProductsAsync(); 

    }
}
