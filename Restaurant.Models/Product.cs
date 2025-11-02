using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Product : BaseModels
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "In-stock value cannot be negative")]
        public int InStock { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; } = true;

        public decimal? DiscountPrice { get; set; }   
        public TimeSpan? DiscountStart { get; set; } 
        public TimeSpan? DiscountEnd { get; set; }   

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int PreparationTime { get; set; }

        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }



     
        //public bool IsHappyHour => DateTime.Now.TimeOfDay >= new TimeSpan(15, 0, 0) && DateTime.Now.TimeOfDay <= new TimeSpan(17, 0, 0);
    }
}
