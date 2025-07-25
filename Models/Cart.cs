namespace ZOLShop.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public string CustomerID { get; set; } = string.Empty;
        public ApplicationUser Customer { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}