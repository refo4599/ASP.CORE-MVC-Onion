using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _repo.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await _repo.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public decimal GetFinalPrice(Product product)
        {
            var now = DateTime.Now.TimeOfDay;
            var happyHourStart = new TimeSpan(9, 0, 0);
            var happyHourEnd = new TimeSpan(2, 0, 0);

            decimal finalPrice = product.Price;

            //  Happy Hour Discount
            if (now >= happyHourStart && now <= happyHourEnd)
            {
                finalPrice = Math.Round(product.Price * 0.8m, 2); // 20% off
            }
         
            else if (product.Price >= 100)
            {
                finalPrice = Math.Round(product.Price * 0.9m, 2); // 10% off
            }

            return finalPrice;
        }

        public bool IsHappyHourNow()
        {
            var now = DateTime.Now.TimeOfDay;
            return now >= new TimeSpan(20, 0, 0) && now <= new TimeSpan(23, 0, 0);
        }
    }
}
