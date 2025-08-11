using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZOLShop.Data;
using ZOLShop.Models;

namespace ZOLShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        // Products Management
        public async Task<IActionResult> Products() => View(await _context.Products.Include(p => p.Category).ToListAsync());

        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile)
        {
            ModelState.Remove("Category");
            ModelState.Remove("CartDetails");
            ModelState.Remove("Reviews");
            
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine("wwwroot/images", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = fileName;
                }
                else
                {
                    product.Image = "placeholder.jpg";
                }
                
                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Products");
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            ModelState.Remove("Category");
            ModelState.Remove("CartDetails");
            ModelState.Remove("Reviews");
            
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == product.ProductID);
                
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine("wwwroot/images", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = fileName;
                    
                    // Delete old image if it exists and is not placeholder
                    if (existingProduct != null && !string.IsNullOrEmpty(existingProduct.Image) && existingProduct.Image != "placeholder.jpg")
                    {
                        var oldImagePath = Path.Combine("wwwroot/images", existingProduct.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
                else if (existingProduct != null)
                {
                    product.Image = existingProduct.Image;
                }
                
                product.UpdatedAt = DateTime.Now;
                product.CreatedAt = existingProduct?.CreatedAt ?? DateTime.Now;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Products");
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Products");
        }

        // Users Management
        public async Task<IActionResult> Users() => View(await _context.Users.OrderBy(u => u.FullName).ToListAsync());

        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(ApplicationUser model)
        {
            var user = await _context.Users.FindAsync(model.Id);
            if (user != null)
            {
                user.FullName = model.FullName;
                user.Address = model.Address;
                await _context.SaveChangesAsync();
                TempData["Success"] = "User updated successfully";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LockoutEnd = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now 
                    ? null 
                    : DateTime.Now.AddYears(100);
                await _context.SaveChangesAsync();
                TempData["Success"] = user.LockoutEnd.HasValue ? "User locked successfully" : "User unlocked successfully";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null && user.Email != "admin@zolshop.com")
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully";
            }
            else
            {
                TempData["Error"] = "Cannot delete admin user";
            }
            return RedirectToAction("Users");
        }

        // Orders Management
        public async Task<IActionResult> Orders() => View(await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .ToListAsync());

        // Categories Management
        public async Task<IActionResult> Categories() => View(await _context.Categories.ToListAsync());

        public IActionResult CreateCategory() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully";
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated successfully";
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == id);
            if (category != null)
            {
                if (category.Products.Any())
                {
                    TempData["Error"] = "Cannot delete category with existing products";
                }
                else
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Category deleted successfully";
                }
            }
            return RedirectToAction("Categories");
        }

        // Payments Management
        public async Task<IActionResult> Payments() => View(await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync());

        public async Task<IActionResult> PaymentDetails(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .Include(p => p.Order.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(p => p.PaymentID == id);
            if (payment == null) return NotFound();
            return View(payment);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Orders");
        }
        
        // Analytics
        public IActionResult Analytics() => View();
        
        [HttpGet]
        public async Task<IActionResult> GetAnalytics()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            
            var monthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear && o.Status == "Completed")
                .SumAsync(o => o.TotalAmount * 24000);
                
            var monthlyOrders = await _context.Orders
                .CountAsync(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear);
                
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Completed")
                .SumAsync(o => o.TotalAmount * 24000);
                
            var newCustomers = await _context.Users
                .CountAsync(u => u.CreatedAt.Month == currentMonth && u.CreatedAt.Year == currentYear);
            
            // Calculate growth percentages
            var lastMonth = DateTime.Now.AddMonths(-1);
            var lastMonthRevenue = await _context.Orders
                .Where(o => o.OrderDate.Month == lastMonth.Month && o.OrderDate.Year == lastMonth.Year && o.Status == "Completed")
                .SumAsync(o => o.TotalAmount * 24000);
                
            var lastMonthOrders = await _context.Orders
                .CountAsync(o => o.OrderDate.Month == lastMonth.Month && o.OrderDate.Year == lastMonth.Year);
                
            var lastMonthCustomers = await _context.Users
                .CountAsync(u => u.CreatedAt.Month == lastMonth.Month && u.CreatedAt.Year == lastMonth.Year);
            
            var monthlyGrowth = lastMonthRevenue > 0 ? ((monthlyRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 : 0;
            var ordersGrowth = lastMonthOrders > 0 ? ((double)(monthlyOrders - lastMonthOrders) / lastMonthOrders) * 100 : 0;
            var customersGrowth = lastMonthCustomers > 0 ? ((double)(newCustomers - lastMonthCustomers) / lastMonthCustomers) * 100 : 0;
            var totalGrowth = 15.2; // Static for demo
            
            return Json(new {
                monthlyRevenue,
                monthlyOrders,
                totalRevenue,
                newCustomers,
                monthlyGrowth,
                ordersGrowth,
                totalGrowth,
                customersGrowth
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetChartData(string period = "month")
        {
            var revenueData = new { labels = new List<string>(), values = new List<decimal>() };
            var productData = new { labels = new List<string>(), values = new List<int>() };
            
            if (period == "month")
            {
                // Last 12 months revenue
                for (int i = 11; i >= 0; i--)
                {
                    var date = DateTime.Now.AddMonths(-i);
                    var revenue = await _context.Orders
                        .Where(o => o.OrderDate.Month == date.Month && o.OrderDate.Year == date.Year && o.Status == "Completed")
                        .SumAsync(o => o.TotalAmount * 24000);
                    
                    revenueData.labels.Add(date.ToString("MM/yyyy"));
                    revenueData.values.Add(revenue);
                }
            }
            
            // Top 5 products
            var topProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => od.Product.Name)
                .Select(g => new { Name = g.Key, Quantity = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync();
                
            productData.labels.AddRange(topProducts.Select(p => p.Name));
            productData.values.AddRange(topProducts.Select(p => p.Quantity));
            
            return Json(new {
                revenue = revenueData,
                products = productData
            });
        }
    }
}