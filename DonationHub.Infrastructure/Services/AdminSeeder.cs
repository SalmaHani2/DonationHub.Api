
using DonationHub.Domain.ValueObject;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Services;

public class AdminSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly SeedAdminOptions _options;


    public AdminSeeder(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole<Guid>> roleManager,IOptions<SeedAdminOptions> options)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options.Value;
    }

    public async Task SeedAsync()
    {
        //const string role = "Admin";

        //if (!await _roleManager.RoleExistsAsync(role))
        //    await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

        string[] roles = { "Admin", "CampaignManager", "Donor" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var admin = await _userManager.FindByEmailAsync(_options.Email);

        if (admin != null)
            return;

        admin = new ApplicationUser(
            _options.Email,
            _options.FullName,
            "en",
            DateTime.UtcNow);

        var result = await _userManager.CreateAsync(admin, _options.Password);

        if (result.Succeeded)
            await _userManager.AddToRoleAsync(admin, "Admin");
    }
}
