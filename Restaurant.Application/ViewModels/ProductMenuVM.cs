using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.ViewModels
{
    public class ProductMenuVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFavourite { get; set; } = false;
        public int Quantity { get; set; } = 1;

        public int InStock { get; set; }
        public bool IsAvailable { get; set; } = true;

        public decimal? DiscountPrice { get; set; }      
        public TimeSpan? DiscountStart { get; set; }     
        public TimeSpan? DiscountEnd { get; set; }
    }
}
