using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;
        
        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // Credit Card, PayPal, etc.
        
        [Required]
        public decimal Amount { get; set; }
        
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
        
        [StringLength(100)]
        public string TransactionID { get; set; } = string.Empty;
        
        // Credit Card Info (encrypted in real app)
        [StringLength(20)]
        public string CardNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string CardHolderName { get; set; } = string.Empty;
        
        [StringLength(5)]
        public string ExpiryDate { get; set; } = string.Empty;
    }
}