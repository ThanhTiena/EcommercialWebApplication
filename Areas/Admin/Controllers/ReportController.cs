using Microsoft.AspNetCore.Mvc;

namespace EcommercialWebApplication.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
