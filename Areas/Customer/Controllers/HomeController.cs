using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommercialWebApplication.Controllers
{
    [Area("Customer")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var products = new List<Product>();

            products = await _context.Products.Include(c => c.Category).ToListAsync();
            ViewBag.Categories = _context.Categories.ToList();
            return View(products);
        }
        public async Task<IActionResult> SearchByCategory(int? id)
        {
            var products = new List<Product>();

            products = await _context.Products.Include(c => c.Category).Where(m => m.Category.Id==id).ToListAsync();
            ViewBag.Categories = _context.Categories.ToList();
            return View("Index",products);
        }

        public ActionResult Detail(int? id)
        {
            IDictionary<int, int> cartQuantities = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if (cartQuantities == null)
            {
                cartQuantities = new Dictionary<int, int>();
            }
            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Include(c => c.Category).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            if (!cartQuantities.ContainsKey(product.Id))
            {
                cartQuantities.Add(product.Id, 1);
            }
            HttpContext.Session.Set("Quantities", cartQuantities);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Detail")]        public ActionResult ProductDetail(int? id)
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            IDictionary<int, int> cartQuantities = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if(cartQuantities == null)
            {
                cartQuantities= new Dictionary<int, int>();
            }
            if (list == null)
            {
                list = new List<Product>();
            }
            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            list.Add(product);
            cartQuantities[product.Id] = 1;
            HttpContext.Session.Set("Quantities", cartQuantities);
            HttpContext.Session.Set("products", list);
            return RedirectToAction("Cart", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int? id)
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            IDictionary<int, int> cartQuantities = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if (cartQuantities == null)
            {
                cartQuantities = new Dictionary<int, int>();
            }
            if (list != null)
            {
                var product = list.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    list.Remove(product);
                    if (cartQuantities.ContainsKey(product.Id))
                    {
                        cartQuantities.Remove(product.Id);
                    }
                    HttpContext.Session.Set("Quantities", cartQuantities);
                    HttpContext.Session.Set("products", list);

                }
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveFromCart(int? id)
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            IDictionary<int, int> cartQuantities = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if (cartQuantities == null)
            {
                cartQuantities = new Dictionary<int, int>();
            }
            if (list != null)
            {
                var product = list.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    list.Remove(product);
                    if (cartQuantities.ContainsKey(product.Id))
                    {
                        cartQuantities.Remove(product.Id);
                    }
                    HttpContext.Session.Set("Quantities", cartQuantities);
                    HttpContext.Session.Set("products", list);

                }
            }
            return RedirectToAction(nameof(Cart));
        }

        public IActionResult Cart()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuantity(OrderDetail orderDetail)
        {
            IDictionary<int, int> orderDetails = HttpContext.Session.Get<IDictionary<int, int>>("Quantities");
            if(orderDetails == null)
            {
                orderDetails= new Dictionary<int, int>();
            }
            if(orderDetail == null)
            {
                orderDetail= new OrderDetail();
            }
            if (orderDetails.ContainsKey(orderDetail.ProductId))
            {
                orderDetails[orderDetail.ProductId] = orderDetail.quantity;
            }
            HttpContext.Session.Set("Quantities", orderDetails);
            return RedirectToAction(nameof(Cart));
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}