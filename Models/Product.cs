using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        
        public int CategoryID { get; set; }
        public Category Category { get; set; } = null!;
        
        [StringLength(200)]
        public string Image { get; set; } = "placeholder.jpg";
        
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        public decimal Weight { get; set; }
        
        [StringLength(50)]
        public string Dimensions { get; set; } = string.Empty;
        
        public bool IsFeatured { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}