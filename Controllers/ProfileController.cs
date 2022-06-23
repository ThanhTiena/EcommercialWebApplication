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
    public class ProfileController : Controller
    {
        public readonly SignInManager<Customer> _signInManager;
        public readonly ApplicationDbContext _context;
        public readonly UserManager<Customer> _userManager;
        public ProfileController(SignInManager<Customer> signinManager,
            ApplicationDbContext context,
            UserManager<Customer> userManager)
        {
            _context = context;
            _signInManager = signinManager;
            _userManager = userManager;
        }
     /*   public IActionResult Profile()
        {
            return View();
        }*/

        public async Task<IActionResult> Profile()
        {
            var isSignIn = _signInManager.IsSignedIn(User);
            if (isSignIn)
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var userId = currentUser.Id;
                if (userId != 0)
                {
                    var user = await _context.Customers.Include(c => c.Profile)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync(c => c.Id == userId);
                    if (user != null)
                    {
                        return View(user.Profile);
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> ProfileDetail()
        {
            var isSignIn = _signInManager.IsSignedIn(User);
            if (isSignIn)
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var userId = currentUser.Id;
                if (userId != 0)
                {
                    var user = await _context.Customers.Include(c => c.Profile)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync(c => c.Id == userId);
                    if (user != null)
                    {
                        return View(user.Profile);
                    }
                }
            }
            return View();
        }
        public async Task<IActionResult> EditProfile()
        {
            var isSignIn = _signInManager.IsSignedIn(User);
            if (isSignIn)
            {
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var userId = currentUser.Id;
                if (userId != 0)
                {
                    var user = await _context.Customers.Include(c => c.Profile).FirstOrDefaultAsync(c => c.Id == userId);
                    if (user != null)
                    {
                        user.Profile.CustomerId = user.Id;
                        return View(user.Profile);
                    }
                }
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile([Bind("CustomerId,FirstName,LastName,Dob,Address,PhoneNumber,EmailAddress,Nationality")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Customers.Include(c => c.Profile)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(c => c.Id == profile.CustomerId);
                if (user != null)
                {
                    _context.Update(profile);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(ProfileDetail));
                }
            }
            return View();
        }
    }
}
