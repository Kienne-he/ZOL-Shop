using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZOLShop.Data;
using ZOLShop.Models;

namespace ZOLShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await GetOrCreateCart(userId!);
            
            var cartDetails = await _context.CartDetails
                .Include(cd => cd.Product)
                .ThenInclude(p => p.Category)
                .Where(cd => cd.CartID == cart.CartID)
                .ToListAsync();

            return View(cartDetails);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "Please login to add items to cart" });

                var cart = await GetOrCreateCart(userId);
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                    return Json(new { success = false, message = "Product not found" });

                if (product.Stock < quantity)
                    return Json(new { success = false, message = "Not enough stock available" });

                var existingItem = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.CartID == cart.CartID && cd.ProductID == productId);

                if (existingItem != null)
                {
                    if (existingItem.Quantity + quantity > product.Stock)
                        return Json(new { success = false, message = "Not enough stock available" });
                    
                    existingItem.Quantity += quantity;
                    existingItem.Price = product.Price * existingItem.Quantity;
                }
                else
                {
                    _context.CartDetails.Add(new CartDetail
                    {
                        CartID = cart.CartID,
                        ProductID = productId,
                        Quantity = quantity,
                        Price = product.Price * quantity
                    });
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Added to cart successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding to cart: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartDetailId, int quantity)
        {
            var cartDetail = await _context.CartDetails
                .Include(cd => cd.Product)
                .FirstOrDefaultAsync(cd => cd.CartDetailID == cartDetailId);

            if (cartDetail == null || quantity <= 0)
                return Json(new { success = false });

            if (quantity > cartDetail.Product.Stock)
                return Json(new { success = false, message = "Not enough stock" });

            cartDetail.Quantity = quantity;
            cartDetail.Price = cartDetail.Product.Price * quantity;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartDetailId)
        {
            var cartDetail = await _context.CartDetails.FindAsync(cartDetailId);
            if (cartDetail != null)
            {
                _context.CartDetails.Remove(cartDetail);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            return RedirectToAction("Checkout", "Payment");
        }

        private async Task<Cart> GetOrCreateCart(string userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerID == userId);
            if (cart == null)
            {
                cart = new Cart { CustomerID = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }
    }
}