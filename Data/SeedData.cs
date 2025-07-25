using Microsoft.AspNetCore.Identity;
using ZOLShop.Models;

namespace ZOLShop.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Roles only (no categories, products, or users)
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("Customer"))
                await roleManager.CreateAsync(new IdentityRole("Customer"));
        }
    }
}