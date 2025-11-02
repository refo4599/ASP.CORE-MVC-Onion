using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.BackgroundServices
{
    public class OrderStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderStatusBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    var allOrders = await orderService.GetOrdersForAdminAsync();
                    var now = DateTime.Now;

                    foreach (var order in allOrders)
                    {
                       
                        if (order.Status == "Pending")
                        {
                            if ((now - order.CreatedAt).TotalMinutes >= 1)
                            {
                                await orderService.UpdateStatusAsync(order.Id, "Preparing");
                            }
                        }

                        // Time Of Replace  Status 
                
                        else if (order.Status == "Preparing")
                        {
                            int maxPrepTime = order.Items.Any()
                                ? order.Items.Max(i => i.Product.PreparationTime)
                                : 0;

                            if ((now - order.CreatedAt).TotalMinutes >= (1 + maxPrepTime))
                            {
                                await orderService.UpdateStatusAsync(order.Id, "Ready");
                            }
                        }

                      
                        else if (order.Status == "Ready" && order.OrderType == "Delivery")
                        {
                         
                            // Complete 

                            if ((now - order.UpdatedAt).TotalMinutes >= 7)
                            {
                                await orderService.UpdateStatusAsync(order.Id, "Completed");
                                await orderService.RequestFeedbackAsync(order.Id); 
                            }
                        }
                    }
                }

            
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
