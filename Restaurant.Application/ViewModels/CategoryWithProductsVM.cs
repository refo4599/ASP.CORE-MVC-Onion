using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.ViewModels
{
    public class CategoryWithProductsVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ProductMenuVM> Products { get; set; } = new List<ProductMenuVM>();
    }
}
