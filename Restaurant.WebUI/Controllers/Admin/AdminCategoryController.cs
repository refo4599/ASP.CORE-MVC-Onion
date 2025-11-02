using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers.Admin
{
    // 
    [Authorize(Roles = "Admin,Staff")]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //     Dashboard 
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // Create View
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }


        // Create 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            // Valdation
            if (!await _categoryService.IsNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError(nameof(category.Name), " This name already exists.");
                return View(category);
            }

            await _categoryService.AddAsync(category);
            TempData["Message"] = " Category added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            if (!await _categoryService.IsNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError(nameof(category.Name), " This name already exists.");
                return View(category);
            }

            await _categoryService.UpdateAsync(category);
            TempData["Message"] = " Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteAsync(id);
            TempData["Message"] = " Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
