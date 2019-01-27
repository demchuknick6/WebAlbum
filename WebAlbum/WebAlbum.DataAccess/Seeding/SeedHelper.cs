using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebAlbum.DomainModel;

namespace WebAlbum.DataAccess.Seeding
{
    internal static class SeedHelper
    {
        public static void Seed(DatabaseContext context)
        {
            context.Roles.AddOrUpdate(x => x.Name,
                new IdentityRole(Role.Admin),
                new IdentityRole(Role.User));
            context.SaveChanges();

            const string defaultAdminName = "Mykola";
            const string defaultAdminEmail = "admin@admin.com";
            const string defaultAdminPassword = "12345678";

            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var admin = new ApplicationUser { UserName = defaultAdminName,
                Email = defaultAdminEmail, EmailConfirmed = true};
            
            if (!context.Users.Any(u => u.UserName == defaultAdminName))
            {
                var result = manager.Create(admin, defaultAdminPassword);
                if (result.Succeeded)
                    manager.AddToRole(admin.Id, Role.Admin);
            }
            else
            {
                var user = context.Users.Single(u => u.UserName.Equals(defaultAdminName, 
                    StringComparison.CurrentCultureIgnoreCase));
                if (user != null)
                    manager.AddToRole(user.Id, Role.Admin);
            }
            
            context.SaveChanges();
        }
    }
}
