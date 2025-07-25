using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; } = string.Empty;
        public ApplicationUser Customer { get; set; } = null!;
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
    
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;
        
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
        
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}