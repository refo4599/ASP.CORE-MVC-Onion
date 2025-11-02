using Microsoft.EntityFrameworkCore;
using Restaurant.Infrastructure.Data;
using Restaurant.Models;
using Restaurant.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Restaurant.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly RestaurantContext _context;

        public ProductRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.Include(p => p.Category).ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products.Include(p => p.Category)
                                   .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetAvailabilityAsync(int productId, bool isAvailable)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsAvailable = isAvailable;
                await _context.SaveChangesAsync();
            }
        }

    
        public async Task DecreaseStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.InStock -= quantity;
                if (product.InStock <= 0)
                {
                    product.InStock = 0;
                    product.IsAvailable = false;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAvailableAsync()
        {
            return await _context.Products
                .Where(p => p.IsAvailable)
                .ToListAsync();
        }
    }
}
