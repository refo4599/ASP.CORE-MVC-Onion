namespace Restaurant.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }       
        public AppUser User { get; set; }

        public decimal Subtotal { get; set; }          
        public decimal TaxAmount { get; set; }      
        public decimal Discount { get; set; }          
        public decimal TotalPrice { get; set; }       


      
        public string Status { get; set; } = "Pending";
        public string OrderType { get; set; }

        public string? DeliveryAddress { get; set; }
        public string? PhoneNumber { get; set; }

        public int? EstimatedDeliveryMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ReadyAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsFeedbackRequested { get; set; } = false;

        public List<OrderItems> Items { get; set; } = new();
    }
}
