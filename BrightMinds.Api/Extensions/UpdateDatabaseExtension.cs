using BrightMinds.Core.Models;
using BrightMinds.Services.ApplicationServicesConfigrations;
using Microsoft.AspNetCore.Identity;
using Blendd.Infrastructure.DataAccess;
using Library.Api.Extensions;
using Blendd.Api.Extensions;

namespace BrightMinds.Api.Extensions
{
    public static class UpdateDatabaseExtension
    {
        public async static Task<WebApplication>UpdateDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            await ApplyMigration.ApplyMigrationsAsync(scope);
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

               await app.SeedAppData(context);
            await app.SeedUsersAndRoles(userManager, roleManager);
            return app;
        }
    }
}
