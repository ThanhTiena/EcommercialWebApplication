using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommercialWebApplication.Controllers
{
    public class AccountController : Controller
    {
        public readonly UserManager<Customer> _userManager;
        public readonly RoleManager<IdentityRole<int>> _roleManager;
        public readonly SignInManager<Customer> _signInManager;

        public AccountController(UserManager<Customer> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            await _roleManager.CreateAsync(new IdentityRole<int>("Customer"));
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Account model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var _user = await _userManager.FindByNameAsync(model.UserName);
                    HttpContext.Session.Set("USERID",_user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid login attemp");
            return View(model);

        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account registerModel)
        {
            Customer user = new Customer();
            {
                user.UserName = registerModel.UserName;
                user.Email = registerModel.Email;
                user.PhoneNumber = registerModel.PhoneNumber;
            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            
            if (result.Succeeded)
            {
               
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                ViewData["Error"] = "PasswordTooShort,PasswordRequiresNonAlphanumeric,PasswordRequiresLower,PasswordRequiresUpper";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
