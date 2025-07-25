using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
        
        public string CustomerID { get; set; } = string.Empty;
        public ApplicationUser Customer { get; set; } = null!;
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;
        
        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}