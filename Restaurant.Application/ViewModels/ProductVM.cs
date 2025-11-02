using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace Restaurant.Application.ViewModels;
    public class ProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

    [Required(ErrorMessage = "In-stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "In-stock cannot be negative")]
    [Display(Name = "In Stock")]
    public int InStock { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        //  Dropdown list of categories (used in Create & Edit)
        public SelectList? Categories { get; set; }

        // Image upload
        [Display(Name = "Upload Product Image")]
        public IFormFile? Image { get; set; }

        // Existing image when editing
        public string? ExistingImageUrl { get; set; }


        

    }
