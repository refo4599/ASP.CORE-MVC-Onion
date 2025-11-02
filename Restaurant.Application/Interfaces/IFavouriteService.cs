using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IFavouriteService
    {
        Task<bool> ToggleFavouriteAsync(int productId, string userId);
        Task<bool> IsFavouriteAsync(int productId, string userId);
        Task<List<Product>> GetFavouritesAsync(string userId);
    }
}
