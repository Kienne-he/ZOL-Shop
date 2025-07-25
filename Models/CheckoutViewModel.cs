using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class CheckoutViewModel
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}