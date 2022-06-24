using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace EcommercialWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Statistic()
        {
            IDictionary<String, decimal> data = new Dictionary<String, decimal>();
            var orders = await _context.Orders.OrderByDescending(d => d.OrderDate).ToListAsync();
            orders.Reverse();
            var loadsize = 7;
            foreach (var order in orders)
            {
                if (!data.ContainsKey(DateOnly.FromDateTime(order.OrderDate).ToString()))
                {
                    data[DateOnly.FromDateTime(order.OrderDate).ToString()] = order.Total;
                }
                else
                {
                    data[DateOnly.FromDateTime(order.OrderDate).ToString()] += order.Total;
                }
                if(data.Count == loadsize) { break; }
            }
            ViewBag.Data = data;
            return View();
        }
    }
}
