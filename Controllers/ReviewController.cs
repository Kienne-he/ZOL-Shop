using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZOLShop.Data;
using ZOLShop.Models;

namespace ZOLShop.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = _userManager.GetUserId(User);
            
            // Check if user already reviewed this product
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductID == productId && r.CustomerID == userId);

            if (existingReview != null)
                return Json(new { success = false, message = "You have already reviewed this product" });

            var review = new Review
            {
                ProductID = productId,
                CustomerID = userId!,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Review added successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = _userManager.GetUserId(User);
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewID == reviewId && r.CustomerID == userId);

            if (review == null)
                return Json(new { success = false, message = "Review not found" });

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Review deleted successfully" });
        }
    }
}