using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteRepository _favRepo;

        public FavouriteService(IFavouriteRepository favRepo)
        {
            _favRepo = favRepo;
        }

        public async Task<bool> ToggleFavouriteAsync(int productId, string userId)
        {
            var existing = await _favRepo.GetByUserAndProductAsync(userId, productId);
            if (existing == null)
            {
                await _favRepo.AddAsync(new Favourite { ProductId = productId, UserId = userId });
                return true; // added
            }
            else
            {
                await _favRepo.DeleteAsync(existing);
                return false; // removed
            }
        }

        public async Task<bool> IsFavouriteAsync(int productId, string userId)
        {
            var existing = await _favRepo.GetByUserAndProductAsync(userId, productId);
            return existing != null;
        }

        public async Task<List<Product>> GetFavouritesAsync(string userId)
        {
            return await _favRepo.GetFavouritesForUserAsync(userId);
        }
    }
}
