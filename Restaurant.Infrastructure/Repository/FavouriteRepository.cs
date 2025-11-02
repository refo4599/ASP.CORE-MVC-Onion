using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly RestaurantContext _context;

        public FavouriteRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<Favourite?> GetByUserAndProductAsync(string userId, int productId)
        {
            return await _context.Favourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        }

        public async Task<List<Product>> GetFavouritesForUserAsync(string userId)
        {
            return await _context.Favourites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product)
                .Select(f => f.Product)
                .ToListAsync();
        }

        public async Task AddAsync(Favourite fav)
        {
            await _context.Favourites.AddAsync(fav);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Favourite fav)
        {
            _context.Favourites.Remove(fav);
            await _context.SaveChangesAsync();
        }
    }
}
