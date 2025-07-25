using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZOLShop.Data;
using ZOLShop.Models;

namespace ZOLShop.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CustomerID == userId);

            if (cart == null || !cart.CartDetails.Any())
                return RedirectToAction("Index", "Cart");

            var model = new Models.CheckoutViewModel
            {
                CartDetails = cart.CartDetails.ToList(),
                TotalAmount = cart.CartDetails.Sum(cd => cd.Price),
                ShippingAddress = (await _userManager.FindByIdAsync(userId!))!.Address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Checkout", model);

            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CustomerID == userId);

            if (cart == null || !cart.CartDetails.Any())
                return RedirectToAction("Index", "Cart");

            // Create Order
            var order = new Order
            {
                CustomerID = userId!,
                OrderDate = DateTime.Now,
                TotalAmount = cart.CartDetails.Sum(cd => cd.Price),
                Status = "Pending",
                ShippingAddress = model.ShippingAddress
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create Order Details
            foreach (var cartDetail in cart.CartDetails)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = cartDetail.ProductID,
                    Quantity = cartDetail.Quantity,
                    Price = cartDetail.Price
                });

                cartDetail.Product.Stock -= cartDetail.Quantity;
            }

            // Process Payment (Mock)
            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentMethod = model.PaymentMethod,
                Amount = order.TotalAmount,
                PaymentDate = DateTime.Now,
                Status = "Completed",
                TransactionID = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                CardNumber = "****" + model.CardNumber[^4..],
                CardHolderName = model.CardHolderName,
                ExpiryDate = model.ExpiryDate
            };

            _context.Payments.Add(payment);
            order.Status = "Confirmed";

            // Clear cart
            _context.CartDetails.RemoveRange(cart.CartDetails);
            await _context.SaveChangesAsync();

            return View("PaymentSuccess", new { Order = order, Payment = payment });
        }
    }



    public class PaymentViewModel
    {
        [Required]
        public string PaymentMethod { get; set; } = "Credit Card";
        
        [Required]
        public string CardNumber { get; set; } = string.Empty;
        
        [Required]
        public string CardHolderName { get; set; } = string.Empty;
        
        [Required]
        public string ExpiryDate { get; set; } = string.Empty;
        
        [Required]
        public string CVV { get; set; } = string.Empty;
        
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
    }
}