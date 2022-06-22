using System.ComponentModel.DataAnnotations;

namespace EcommercialWebApplication.Models
{
    public class Account
    {
        [RegularExpression(@"^[a-z_0-9]+$", ErrorMessage = "Username contains letters a-z, numbers 0-9 and no spaces or symbols")]
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
           ErrorMessage = "Password must contain at least eight characters, at least one number and both lower and uppercase letters and special characters")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }
        [EmailAddress]

        [RegularExpression(@"^[\w-\.]+@[a-z]+(\.[a-z]+)+$",
            ErrorMessage = "Email is not valid")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [RegularExpression(@"^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$",
            ErrorMessage = "Wrong phone number format")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }
        public string CurrentPassword { get; set; }
    }
}
