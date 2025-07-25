using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZOLShop.Data;
using ZOLShop.Models;

namespace ZOLShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Json(new List<object>());

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query) || p.Category.Name.Contains(query))
                .Take(10)
                .Select(p => new
                {
                    id = p.ProductID,
                    name = p.Name,
                    price = p.Price,
                    category = p.Category.Name,
                    image = p.Image,
                    stock = p.Stock
                })
                .ToListAsync();

            return Json(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetPriceRange()
        {
            var products = await _context.Products.ToListAsync();
            var result = new
            {
                min = products.Any() ? products.Min(p => p.Price) : 0,
                max = products.Any() ? products.Max(p => p.Price) : 0
            };
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> QuickView(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            var result = new
            {
                id = product.ProductID,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                stock = product.Stock,
                category = product.Category.Name,
                image = product.Image,
                averageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                reviewCount = product.Reviews.Count
            };

            return Json(result);
        }
    }
}