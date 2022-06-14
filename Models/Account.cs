using System.ComponentModel.DataAnnotations;

namespace EcommercialWebApplication.Models
{
    public class Account
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CurrentPassword { get; set; }
        public bool RememberMe { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
