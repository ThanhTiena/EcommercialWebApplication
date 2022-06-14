using EcommercialWebApplication.Data;
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

        public OrderController (ApplicationDbContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CheckOut(Order order)
        {
            decimal total = 0;
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            IDictionary<int,int> orderDetails = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if (list != null && orderDetails != null)
            {
                foreach(var product in list)
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

            order.UserId = HttpContext.Session.Get<int>("USERID");
            order.OrderDate = DateTime.Now;
            order.Status = OrderStatus.InProgress;
            order.Total = total;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Set("products", new List<Product>());
            return RedirectToAction("Index", "Home", new { area = "Customer" }); ;
        }

    }
}
