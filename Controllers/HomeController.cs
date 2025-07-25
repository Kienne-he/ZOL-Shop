using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZOLShop.Data;
using ZOLShop.Models;

namespace ZOLShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, decimal? minPrice, decimal? maxPrice, string sortBy, bool inStock = false)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString) || (p.Category != null && p.Category.Name.Contains(searchString)));
                ViewBag.SearchString = searchString;
            }

            // Category filter
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            // Price range filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice);
                ViewBag.MinPrice = minPrice;
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice);
                ViewBag.MaxPrice = maxPrice;
            }

            // Stock filter
            if (inStock)
            {
                products = products.Where(p => p.Stock > 0);
                ViewBag.InStock = inStock;
            }

            // Sorting
            ViewBag.SortBy = sortBy;
            products = sortBy switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "rating" => products.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
                "newest" => products.OrderByDescending(p => p.ProductID),
                _ => products.OrderBy(p => p.Name)
            };

            // Get price range for filter
            var allProducts = await _context.Products.ToListAsync();
            ViewBag.MaxProductPrice = allProducts.Any() ? allProducts.Max(p => p.Price) : 0;
            ViewBag.MinProductPrice = allProducts.Any() ? allProducts.Min(p => p.Price) : 0;

            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}