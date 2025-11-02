using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Favourite
    {
        public int Id { get; set; }

       
        public string UserId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
