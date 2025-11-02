using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Models;
using System.Threading.Tasks;

namespace Restaurant.WebUI.Controllers
{
    //[Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // 
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

       
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        
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
            TempData["Message"] = " The category has been added successfully";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            // Valdation
            if (!await _categoryService.IsNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError(nameof(category.Name), " This name already exists.");
                return View(category);
            }

            await _categoryService.UpdateAsync(category);
            TempData["Message"] = " The category has been Update successfully";
            return RedirectToAction(nameof(Index));
        }

        // 
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // 
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteAsync(id);
            TempData["Message"] = " The category has been successfully deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
