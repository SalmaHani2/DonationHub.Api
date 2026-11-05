using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Seed;

public class AdminSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly SeedAdminOptions _options;

    private static readonly string[] Roles =
    {
        "Admin",
        "CampaignManager",
        "Donor"
    };

    public AdminSeeder(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole<Guid>> roleManager,IOptions<SeedAdminOptions> options)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options.Value;
    }

    public async Task SeedAsync(IServiceProvider services)
    {
        // Create roles if they do not exist.
        foreach (var role in Roles)
        {
            var roleExists =
                await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create role: {role}");
                }
            }
        }

        // Find the initial Admin.
        var admin = await _userManager.FindByEmailAsync(
            _options.Email);

        if (admin == null)
        {
            admin = new ApplicationUser(
                _options.Email,
                _options.FullName,
                "en",
                DateTime.UtcNow);

            var createResult = await _userManager.CreateAsync(
                admin,
                _options.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(error => error.Code));

                throw new InvalidOperationException(
                    $"Could not create the initial Admin: {errors}");
            }
        }

        // Ensure the user has the Admin role.
        var isAdmin = await _userManager.IsInRoleAsync(
            admin,
            "Admin");

        if (!isAdmin)
        {
            var roleResult = await _userManager.AddToRoleAsync(
                admin,
                "Admin");

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not assign the Admin role.");
            }
        }
    }
}