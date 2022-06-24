using EcommercialWebApplication.Data;
using EcommercialWebApplication.Models;
using EcommercialWebApplication.Utility;
using Microsoft.AspNetCore.Authorization;
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

        /*public async Task<IActionResult> Index()
        {
            await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            await _roleManager.CreateAsync(new IdentityRole<int>("Customer"));
            return View();
        }*/

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Account model)
        {
            if (!ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var _user = await _userManager.FindByNameAsync(model.UserName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attemp");
                }
            }
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
                var errors = "";
                foreach (var error in result.Errors.ToList())
                {
                    errors += error.Code + " ";
                }
                ViewData["Errors"] = errors;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(Account account)
        {
            var result = await _userManager.FindByEmailAsync(account.Email);
            if (result != null)
            {
                HttpContext.Session.Set("Email", account.Email);
                return RedirectToAction(nameof(RecoverPassword));
            }
            ViewData["Error"] = "Email Has Not Existed";
            return View();
        }

        [HttpGet]
        public IActionResult RecoverPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword([Bind("Password, ConfirmPassword")] Account model)
        {
            if (model == null)
            {
                ViewData["Error"] = "Please Input Password";
                return View();
            }
            if (model.Password.Equals(model.ConfirmPassword))
            {


                var email = HttpContext.Session.Get<String>("Email");
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
            }
            ViewData["Error"] = "Please Input The Same Password";
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(Account account)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var userId = currentUser.Id;
                if (userId != 0)
                {
                    var oldUser = await _userManager.FindByIdAsync(userId.ToString());
                    if (oldUser != null)
                    {
                        var result = await _userManager.ChangePasswordAsync(oldUser, account.CurrentPassword, account.Password);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Profile", "Profile");
                        }
                    }
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerInformation([Bind("FirstName,LastName,Dob,Address,Nationality")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                if (profile == null)
                {
                    return NotFound();
                }
                var userId = HttpContext.Session.Get<int>("USERID");
                if (userId != 0)
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user == null || userId == 0)
                    {
                        return NotFound();
                    }
                    var _profile = _context.Profiles.FirstOrDefault(p => p.FirstName == profile.FirstName && p.LastName == profile.LastName);
                    if (_profile == null)
                    {
                        profile.CustomerId = user.Id;
                        profile.PhoneNumber = user.PhoneNumber;
                        profile.EmailAddress = user.Email;
                        _context.Profiles.Add(profile);
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Account");
                    }
                }

            }

            ViewData["Error"] = "User has been existed";
            return View(profile);
        }
    }
}
