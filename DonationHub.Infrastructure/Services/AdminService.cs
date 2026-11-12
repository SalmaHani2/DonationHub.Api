
using DonationHub.Application.Admin.Admin;
using DonationHub.Application.Admin.Interfaces;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreateStaffResponse> CreateStaffAsync(
        CreateStaffRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "Admin.EmailAlreadyExists");
        }

        // Create staff user
        var user = new ApplicationUser(
            email,
            request.FullName.Trim(),
            "en",
            DateTime.UtcNow);

        var createResult = await _userManager.CreateAsync(
            user,
            request.TemporaryPassword);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    createResult.Errors.Select(error => error.Description)));
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(
            user,
            request.Role);

        if (!roleResult.Succeeded)
        {
            // Remove the created user if role assignment fails
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    roleResult.Errors.Select(error => error.Description)));
        }

        return new CreateStaffResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = request.Role
        };
    }
}
