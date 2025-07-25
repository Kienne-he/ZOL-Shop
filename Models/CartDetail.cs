using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class CartDetail
    {
        public int CartDetailID { get; set; }
        public int CartID { get; set; }
        public Cart Cart { get; set; } = null!;
        
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
    }
}