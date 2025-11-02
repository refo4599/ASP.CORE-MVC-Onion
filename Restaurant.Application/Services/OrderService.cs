using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<Order> CreateOrderAsync(
            string userId,
            List<OrderItems> cartItems,
            string orderType,
            string? deliveryAddress,
            string? phoneNumber)
        {
            var subtotal = cartItems.Sum(i => i.Price * i.Quantity);
            var tax = subtotal * 0.085m;
            var discount = 0m;
            var total = subtotal + tax - discount;

            
            int maxPrepTime = cartItems.Any() ? cartItems.Max(i => i.Product.PreparationTime) : 0;

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                OrderType = orderType ?? "Delivery",
                Subtotal = subtotal,
                TaxAmount = tax,
                Discount = discount,
                TotalPrice = total,
                Items = cartItems,
                DeliveryAddress = deliveryAddress,
                PhoneNumber = phoneNumber,
                EstimatedDeliveryMinutes = maxPrepTime + 30  /////// 30 minute buffer for delivery time
            };

            await _orderRepository.AddAsync(order);
            return order;
        }


        public async Task<List<Order>> GetOrdersForAdminAsync()
            => await _orderRepository.GetOrdersWithItemsAsync();

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
            => await _orderRepository.GetOrdersByUserIdAsync(userId);

        public async Task<Order?> GetOrderByIdAsync(int id)
            => await _orderRepository.GetByIdAsync(id);

        public async Task UpdateStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return;

            if (order.Status != newStatus)
            {
                order.Status = newStatus;
                order.UpdatedAt = DateTime.Now;

                if (newStatus == "Ready" && !order.ReadyAt.HasValue)
                    order.ReadyAt = DateTime.Now;

                await _orderRepository.UpdateOrderAsync(order);
            }
        }

        public async Task RequestFeedbackAsync(int orderId)
        {
            await _orderRepository.RequestFeedbackAsync(orderId);
        }



        public async Task DeleteAsync(int orderId)
            => await _orderRepository.DeleteAsync(orderId);

        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status) || status.Equals("All", StringComparison.OrdinalIgnoreCase))
                return await _orderRepository.GetOrdersWithItemsAsync();

            return await _orderRepository.GetOrdersByStatusAsync(status);
        }
    }
}
