using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using ZOLShop.Data;
using ZOLShop.Models;

public class CreateProductModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateProductModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product Product { get; set; } = new Product();

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public void OnGet()
    {
        // Load categories for dropdown if needed
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (ImageFile != null && ImageFile.Length > 0)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }
            Product.Image = fileName;
        }

        _context.Products.Add(Product);
        await _context.SaveChangesAsync();
        return RedirectToPage("Products");
    }
}