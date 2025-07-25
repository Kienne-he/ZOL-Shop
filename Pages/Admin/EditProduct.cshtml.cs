using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using ZOLShop.Data;
using ZOLShop.Models;

public class EditProductModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditProductModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product? Product { get; set; } // Made nullable to fix CS8618

    [BindProperty]
    public IFormFile? ImageFile { get; set; } // Made nullable to fix CS8618

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Product = await _context.Products.FindAsync(id); // CS8601 fixed: Product is nullable
        if (Product == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var productToUpdate = await _context.Products.FindAsync(Product!.ProductID);
        if (productToUpdate == null)
        {
            return NotFound();
        }

        // Update product fields
        productToUpdate.Name = Product.Name;
        productToUpdate.Description = Product.Description;
        productToUpdate.Price = Product.Price;
        productToUpdate.Stock = Product.Stock;
        productToUpdate.CategoryID = Product.CategoryID;

        // Handle image upload
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }
            productToUpdate.Image = fileName;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("Products");
    }
}