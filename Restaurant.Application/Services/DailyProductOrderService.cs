using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class DailyProductOrderService : IDailyProductOrderService
    {
        private readonly IDailyProductOrderRepository _repo;

        public DailyProductOrderService(IDailyProductOrderRepository repo)
        {
            _repo = repo;
        }

        // Service
        public async Task TrackOrderAsync(int productId)
        {
            await _repo.TrackOrderAsync(productId);
        }


        public async Task<List<DailyOrderVM>> GetTodayOrdersAsync()
        {
            var data = await _repo.GetTodayOrdersAsync();
            return data.Select(d => new DailyOrderVM
            {
                ProductName = d.Product.Name,
                Count = d.Count
            }).ToList();
        }
    }
}
