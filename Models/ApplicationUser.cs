using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ZOLShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ProfileImage { get; set; } = "default-avatar.jpg";
        
        public DateTime DateOfBirth { get; set; }
        
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string PostalCode { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? LastLoginAt { get; set; }
        
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}