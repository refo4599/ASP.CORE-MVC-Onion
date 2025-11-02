using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemVM> Items { get; set; } = new List<CartItemVM>();
        public decimal Subtotal => Items.Sum(i => i.Price * i.Quantity);
        public decimal Tax => Subtotal * 0.085m;
        public decimal Discount { get; set; } = 0m;

        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    }
}
