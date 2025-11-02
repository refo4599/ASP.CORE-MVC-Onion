using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IDailyProductOrderRepository
    {
        Task TrackOrderAsync(int productId);
        Task<List<DailyProductOrder>> GetTodayOrdersAsync();
    }
}
