using Restaurant.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IDailyProductOrderService
    {
        Task TrackOrderAsync(int productId);
        Task<List<DailyOrderVM>> GetTodayOrdersAsync();
    }
}
