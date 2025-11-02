using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestaurantContext _context;

        public OrderRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.OrderType))
                order.OrderType = "Delivery";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null) return;

            existingOrder.Status = order.Status;
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetOrdersWithItemsAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return;

            _context.OrderItems.RemoveRange(order.Items);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return;

           
            if (order.Status != newStatus)
            {
                order.Status = newStatus;
                order.UpdatedAt = DateTime.Now;

                if (newStatus == "Ready" && !order.ReadyAt.HasValue)
                {
                    order.ReadyAt = DateTime.Now;
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RequestFeedbackAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return;

            order.IsFeedbackRequested = true;
            await _context.SaveChangesAsync();
        }



        public async Task<int> CountOrdersForProductTodayAsync(int productId)
        {
            var today = DateTime.Today;

            return await _context.OrderItems
                .Where(oi => oi.ProductId == productId && oi.Order.CreatedAt.Date == today)
                .SumAsync(oi => oi.Quantity);
        }
    }
}
