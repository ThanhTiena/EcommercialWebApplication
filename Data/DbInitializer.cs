using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EcommercialWebApplication.Data;
using Microsoft.AspNetCore.Identity;
using EcommercialWebApplication.Models;

namespace EducationalManagement.Data
{
    public static class DbInitializer
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                string[] roles = new string[] { "Admin", "Customer" };

                var newrolelist = new List<IdentityRole<int>>();
                foreach (string role in roles)
                {
                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        newrolelist.Add(new IdentityRole<int>(role));
                    }
                }
                context.Roles.AddRange(newrolelist);
                context.SaveChanges();
            }
        }
    }
}