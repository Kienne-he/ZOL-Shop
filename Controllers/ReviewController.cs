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
        public async Task<IActionResult> AddReview(int productId, int rating, string comment, bool updateOnly = false)
        {
            var userId = _userManager.GetUserId(User);
            
            if (rating > 0)
            {
                // Check for existing rating by this user
                var existingRating = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.ProductID == productId && r.CustomerID == userId && r.Rating > 0);
                
                if (existingRating != null)
                {
                    // Update existing rating
                    existingRating.Rating = rating;
                    existingRating.ReviewDate = DateTime.Now;
                }
                else
                {
                    // Create new rating only
                    var ratingReview = new Review
                    {
                        ProductID = productId,
                        CustomerID = userId!,
                        Rating = rating,
                        Comment = null,
                        ReviewDate = DateTime.Now
                    };
                    _context.Reviews.Add(ratingReview);
                }
            }
            else if (!string.IsNullOrEmpty(comment) && !updateOnly)
            {
                // Only add comment if not rating update
                var commentReview = new Review
                {
                    ProductID = productId,
                    CustomerID = userId!,
                    Rating = 0,
                    Comment = comment,
                    ReviewDate = DateTime.Now
                };
                _context.Reviews.Add(commentReview);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
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