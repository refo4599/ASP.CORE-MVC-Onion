using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using Restaurant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
                await _categoryRepository.DeleteAsync(category);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _categoryRepository.IsNameUniqueAsync(name, excludeId);
        }


        // .Select(...).ToList() ==>Convert From  Entities to ViewModels
        public async Task<List<CategoryWithProductsVM>> GetAllWithProductsAsync()
        {
            var categories = await _categoryRepository.GetAllWithProductsAsync();

            return categories.Select(c => new CategoryWithProductsVM
            {
                Id = c.Id,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductMenuVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    InStock = p.InStock,
                    IsAvailable = p.IsAvailable
                }).ToList()
            }).ToList();
        }
    }
}
