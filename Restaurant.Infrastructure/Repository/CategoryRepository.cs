using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RestaurantContext _context;

        public CategoryRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }


        // Business / Domain Validation Implement in DB 
        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return !await _context.Categories
                .AnyAsync(c => c.Name == name && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.Products.Any(p => p.IsAvailable && p.InStock > 0))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.Products.Any()) 
                .ToListAsync();
        }

    }
}
