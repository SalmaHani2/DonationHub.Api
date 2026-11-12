using DonationHub.Application.Admin.Options;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var options = scope.ServiceProvider
            .GetRequiredService<IOptions<SeedAdminOptions>>()
            .Value;

        if (string.IsNullOrWhiteSpace(options.Email) ||
            string.IsNullOrWhiteSpace(options.FullName) ||
            string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException(
                "SeedAdminOptions is missing required values."
            );
        }

        string[] roles =
        {
            "Admin",
            "CampaignManager",
            "Donor"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role)
                );
            }
        }

        var admin = await userManager.FindByEmailAsync(options.Email);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = options.Email,
                Email = options.Email,
                FullName = options.FullName,
                PreferredLanguage = "en",
                IsActive = true
            };

            var result = await userManager.CreateAsync(
                admin,
                options.Password
            );

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(x => x.Description))
                );
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}