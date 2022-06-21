﻿using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        public OrderController(ApplicationDbContext context,
            UserManager<Customer> userManager,
            SignInManager<Customer> signInManager)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> CheckOut()
        {
            Order order = new Order();
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (currentUser != null)
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(m => m.CustomerId == currentUser.Id);
                    order.ShipName = profile.FullNamae;
                    order.ShipAddress = profile.Address;
                    order.ShipPhoneNumber = profile.PhoneNumber;
                    order.ShipEmail = profile.EmailAddress;
                }
            }
            ViewBag.Coupon = await _context.Coupons.ToListAsync();
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CheckOut([Bind("ShipName,ShipAddress,ShipPhoneNumber,ShipPhoneName,ShipEmail,PaymentMethod")] Order order)
        {
            decimal total = 0;
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            IDictionary<int, int> orderDetails = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if (list != null && orderDetails != null)
            {
                foreach (var product in list)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.ProductId = product.Id;
                    detail.quantity = orderDetails[product.Id];
                    total += product.Price * detail.quantity;
                    order.OrderDetails.Add(detail);

                    var oldProduct = await _context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.Id == product.Id);
                    oldProduct.Stock -= orderDetails[product.Id];
                    _context.Update(oldProduct);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                order.UserId = currentUser.Id;
                order.OrderDate = DateTime.Now;
                order.Status = OrderStatus.InProgress;
                order.Total = total;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Set("products", new List<Product>());
                return RedirectToAction("Index", "Home", new { area = "Customer" }); ;
            }
            else
            {
                return RedirectToAction("Login", "Account");
            } 
        }
    }
}
