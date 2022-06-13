using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommercialWebApplication.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include(c => c.Category).ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(products);
        }

        public ActionResult Detail(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Include(c => c.Category).FirstOrDefault(c=> c.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ActionName("Detail")]
        public ActionResult ProductDetail(int? id)
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
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
            HttpContext.Session.Set("products", list);
            return View(product);
        }

        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            if (list != null)
            {
                var product = list.FirstOrDefault(c => c.Id == id);
                if(product != null)
                {
                    list.Remove(product);
                    HttpContext.Session.Set("products", list);

                }
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveFromCart(int? id)
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            if (list != null)
            {
                var product = list.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    list.Remove(product);
                    HttpContext.Session.Set("products", list);

                }
            }
            return RedirectToAction(nameof(Cart));
        }

        public IActionResult Cart()
        {
            List<Product> list = HttpContext.Session.Get<List<Product>>("products");
            if(list == null)
            {
                list =new List<Product>();
            }

            return View(list);
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