using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IFavouriteRepository
    {
        Task<Favourite?> GetByUserAndProductAsync(string userId, int productId);
        Task<List<Product>> GetFavouritesForUserAsync(string userId);
        Task AddAsync(Favourite fav);
        Task DeleteAsync(Favourite fav);
    }
}
