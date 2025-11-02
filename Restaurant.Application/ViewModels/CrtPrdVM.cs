using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Restaurant.Application.ViewModels
{
    public class CrtPrdVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 999999.99, ErrorMessage = "Price must be between 1 and 999,999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // لعرض القائمة المنسدلة
        public Microsoft.AspNetCore.Mvc.Rendering.SelectList? Categories { get; set; }

        // لرفع الصورة
        [Display(Name = "Upload Image")]
        public IFormFile? Image { get; set; }
    }
}
