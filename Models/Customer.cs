using Microsoft.AspNetCore.Identity;

namespace EcommercialWebApplication.Models
{
    public class Customer: IdentityUser<int>
    {
        public Profile Profile { get; set; }
    }
}
