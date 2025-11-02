using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using Restaurant.Models;

namespace Restaurant.WebUI.Controllers.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;

        public AdminProductController(
            IProductService productService,
            ICategoryService categoryService,
            IWebHostEnvironment environment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _environment = environment;
        }

        
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

       
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProductVM
            {
                Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name")
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", vm.CategoryId);
                return View(vm);
            }

            var product = new Product
            {
                Name = vm.Name,
                Price = vm.Price,
                Quantity = vm.Quantity,
                InStock = vm.InStock,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                ImageUrl = await SaveImageAsync(vm.Image)
            };

            await _productService.AddAsync(product);
            TempData["Message"] = " Product added successfully!";
            return RedirectToAction(nameof(Index));
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                InStock = product.InStock,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ExistingImageUrl = product.ImageUrl,
                Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", product.CategoryId)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", vm.CategoryId);
                return View(vm);
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            if (vm.Image != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                product.ImageUrl = await SaveImageAsync(vm.Image);
            }

            product.Name = vm.Name;
            product.Price = vm.Price;
            product.Quantity = vm.Quantity;
            product.InStock = vm.InStock;
            product.Description = vm.Description;
            product.CategoryId = vm.CategoryId;
            product.IsAvailable = product.InStock > 0;


            await _productService.UpdateAsync(product);
            TempData["Message"] = "✏️ Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["Message"] = "🗑️ Product deleted successfully!";
           
            return RedirectToAction("Index", "AdminProduct");
        }


        // 
        private async Task<string?> SaveImageAsync(IFormFile? image)
        {
            if (image == null || image.Length == 0) return null;

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return "/images/" + uniqueFileName;
        }
    }
}
