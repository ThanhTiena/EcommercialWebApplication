using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommercialWebApplication.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        public readonly SignInManager<Customer> _signInManager;
        public readonly ApplicationDbContext _context;
        public readonly UserManager<Customer> _userManager;
        public HistoryController(SignInManager<Customer> signinManager,
            ApplicationDbContext context,
            UserManager<Customer> userManager)
        {
            _context = context;
            _signInManager = signinManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> HistoryOrders()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (currentUser == null)
                {
                    return NotFound();
                }
                var listOrders = await _context.Orders.Where(m=> m.UserId == currentUser.Id).ToListAsync();
                return View(listOrders);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (currentUser == null)
                {
                    return NotFound();
                }
                var order = await _context.Orders.Include(m => m.OrderDetails)
                                                    .ThenInclude(m => m.Product)
                                                    .FirstOrDefaultAsync(m => m.Id == id && m.UserId == currentUser.Id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            return View(await _context.Orders.Include(m => m.OrderDetails)
                .ThenInclude(m => m.Product)
                .FirstOrDefaultAsync(m=> m.Id == id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Id,Status")]Order order)
        {
            if (ModelState.IsValid)
            {
                var oldOrder = await _context.Orders.FirstOrDefaultAsync(m => m.Id == order.Id);
                oldOrder.Status = order.Status;
                _context.Update(oldOrder);
                _context.SaveChanges();
                return RedirectToAction(nameof(HistoryOrders));

            }
            return View();
        }
    }
}
