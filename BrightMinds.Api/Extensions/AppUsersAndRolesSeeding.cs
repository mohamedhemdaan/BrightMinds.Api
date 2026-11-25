using BrightMinds.Core.Models;
using Microsoft.AspNetCore.Identity;

/* What roles do we have?
 * -
*/

namespace Blendd.Api.Extensions
{
    public static class AppUsersAndRolesSeeding
    {
        public static async Task<WebApplication> SeedUsersAndRoles(this WebApplication application, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var admin = new IdentityRole()
                {
                    Name = "Admin"
                };

                var nightShiftChef = new IdentityRole()
                {
                    Name = "NightShiftChef"
                };

                var morningShiftChef = new IdentityRole()
                {
                    Name = "MorningShiftChef"
                };

                var operationChef = new IdentityRole()
                {
                    Name = "OperationChef"
                };

                var morningShiftWarehouseClerk = new IdentityRole()
                {
                    Name = "MorningShiftWarehouseClerk"
                };

                var nightShiftWarehouseClerk = new IdentityRole()
                {
                    Name = "NightShiftWarehouseClerk"
                };

                var warehouseManager = new IdentityRole()
                {
                    Name = "WarehouseManager"
                };

                await roleManager.CreateAsync(admin);
                await roleManager.CreateAsync(nightShiftChef);
                await roleManager.CreateAsync(morningShiftChef);
                await roleManager.CreateAsync(operationChef);
                await roleManager.CreateAsync(morningShiftWarehouseClerk);
                await roleManager.CreateAsync(nightShiftWarehouseClerk);
                await roleManager.CreateAsync(warehouseManager);
            }


            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    UserName = "mo7amed92",
                    FirstName = "محمد",
                    LastName = "فتحي",
                    Email = "mo7amed92@gmail.com",
                };

                var creator = new AppUser()
                {
                    UserName = "MahmoudNader",
                    FirstName = "Mahmoud",
                    LastName = "Nader",
                    Email = "medonader567@gmail.com"
                };

                var result1 = await userManager.CreateAsync(user, "aboElhanafy123#");
                if (result1.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }

                var result2 = await userManager.CreateAsync(creator, "MahmoudNader123#@!");
                if (result2.Succeeded)
                {
                    await userManager.AddToRoleAsync(creator, "Admin");
                }
            
            }
            return application;
        }
    }
}
