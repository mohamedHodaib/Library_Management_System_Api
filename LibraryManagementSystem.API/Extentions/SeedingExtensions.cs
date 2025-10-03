using LibraryManagementSystem.API;
using LibraryManagementSystem.API.Constants;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions
{
    public static class SeedingExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    logger.LogInformation("Starting database seeding.");
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var configuration = services.GetRequiredService<IConfiguration>();

                    await SeedRolesAndAdminUser(userManager, roleManager, configuration);
                    logger.LogInformation("Database seeding finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred during database seeding: {Message}", ex.Message);
                    // rethrow to prevent silent failure during startup.
                    throw;
                }
            }
        }

        //We seeding the data here to be able access user manager and role manager
        private async static Task SeedRolesAndAdminUser(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            // 1. Create Roles if they don't exist
            if (!await roleManager.RoleExistsAsync(Roles.Admin)) await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            if (!await roleManager.RoleExistsAsync(Roles.Author)) await roleManager.CreateAsync(new IdentityRole(Roles.Author));
            if (!await roleManager.RoleExistsAsync(Roles.Borrower)) await roleManager.CreateAsync(new IdentityRole(Roles.Borrower));

            // 2. Create Admin User if it doesn't exist
            var adminEmail = config["Admin:Email"];
            var adminPassword = config["Admin:Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                throw new InvalidOperationException("Admin user settings are not configured in appsettings.json.");
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                { 
                    PersonId = 6,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true 
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }
        }
    }
}
