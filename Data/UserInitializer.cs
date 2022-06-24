using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EcommercialWebApplication.Data;
using Microsoft.AspNetCore.Identity;
using EcommercialWebApplication.Models;

namespace EducationalManagement.Data
{
    public static class UserInitializer
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (UserManager<Customer> _userManager = serviceProvider.GetRequiredService<UserManager<Customer>>())
            {
                var userlist = new List<Customer>()
            {
                new Customer(){UserName="admin2@gmail.com", PasswordHash= "Abc12345!" },
                new Customer(){UserName ="admin3@gmail.com", PasswordHash="Abc12345!"}
            };

                foreach (var user in userlist)
                {
                    if (!_userManager.Users.Any(r => r.UserName == user.UserName))
                    {
                        var newuser = new Customer { UserName = user.UserName, Email = user.UserName };
                        var result = _userManager.CreateAsync(newuser, user.PasswordHash);
                    }
                }

            }
        }
    }
}