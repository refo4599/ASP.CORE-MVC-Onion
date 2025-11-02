using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public class DailyProductOrderRepository : IDailyProductOrderRepository
    {
        private readonly RestaurantContext _context;

        public DailyProductOrderRepository(RestaurantContext context)
        {
            _context = context;
        }

        // Repository
        public async Task TrackOrderAsync(int productId)
        {
            var today = DateTime.UtcNow.Date;
            var existing = await _context.DailyProductOrders
                .Include(d => d.Product)
                .FirstOrDefaultAsync(d => d.ProductId == productId && d.Date == today);

            if (existing != null)
            {
                existing.Count += 1;
                _context.DailyProductOrders.Update(existing);
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null) return;

                await _context.DailyProductOrders.AddAsync(new DailyProductOrder
                {
                    ProductId = productId,
                    Product = product,
                    Count = 1,
                    Date = today
                });
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<DailyProductOrder>> GetTodayOrdersAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.DailyProductOrders
                .Include(d => d.Product)
                .Where(d => d.Date == today)
                .ToListAsync();
        }
    }
}
