using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommercialWebApplication.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController (ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CheckOut(Order order)
        {
            decimal total = 0;
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            if(list != null)
            {
                foreach(var product in list)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.ProductId = product.Id;
                    total += product.Price;
                    order.OrderDetails.Add(detail);
                }
            }
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
