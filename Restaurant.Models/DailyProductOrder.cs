using System;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class DailyProductOrder
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public DateTime Date { get; set; } 
        public int Count { get; set; } = 0;
    }
}
