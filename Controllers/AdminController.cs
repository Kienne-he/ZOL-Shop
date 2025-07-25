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
    }
}