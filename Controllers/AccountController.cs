using EcommercialWebApplication.Data;
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
        public readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, UserManager<Customer> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
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
                    HttpContext.Session.Set("USERID", _user.Id);
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
                var _user = await _userManager.FindByNameAsync(registerModel.UserName);
                HttpContext.Session.Set("USERID", _user.Id);
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            else
            {
                ViewData["Error"] = "PasswordTooShort,PasswordRequiresNonAlphanumeric,PasswordRequiresLower,PasswordRequiresUpper";
                return View();
            }

            return RedirectToAction("CustomerInformation", "Account");
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(Account account)
        {
            var result = await _userManager.FindByEmailAsync(account.Email);
            if (result != null)
            {
                return RedirectToAction(nameof(ChangePassword), account);
            }
            ViewData["Error"] = "Email Invalid";
            return View();
        }
        [HttpGet]
        public IActionResult ChangePassword(Account model)
        {
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePasswordUpdate(Account account)
        {
            var userId = HttpContext.Session.Get<int>("USERID");
            var oldUser = await _userManager.FindByIdAsync(userId.ToString());
            if (oldUser != null)
            {
                var result = await _userManager.ChangePasswordAsync(oldUser, account.CurrentPassword, account.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult CustomerInformation()
        {
            Profile profile = new Profile();
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> CustomerInformation(Profile profile)
        {
            if (profile == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.Get<int>("USERID");
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || userId == null)
            {
                return NotFound();
            }
            var _profile = _context.Profiles.FirstOrDefault(p => p.FirstName == profile.FirstName && p.LastName == profile.LastName);
            if(_profile == null)
            {
                profile.CustomerId = user.Id;
                profile.PhoneNumber = user.PhoneNumber;
                profile.EmailAddress = user.Email;
                _context.Profiles.Add(profile);
                _context.SaveChanges();
                return RedirectToAction("Login", "Account");

            }
            ViewData["Error"] = "User has been existed";
            return View(profile);
        }
    }
}
