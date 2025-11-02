
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Category:BaseModels
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter category name")]
        [StringLength(100, ErrorMessage = "Max 100 characters allowed")]
        //[Remote(action: "IsUniqueName", controller: "Category", ErrorMessage = "This category name already exists.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "max 100 character")]
        public string? Description { get; set; }


        public List<MenuItems>? MenuItems { get; set; } = new List<MenuItems>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
    