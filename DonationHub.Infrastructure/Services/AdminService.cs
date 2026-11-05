using DonationHub.Application.DTOs.Admin;
using DonationHub.Application.Interfaces;
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

    public async Task<CreateStaffResponse> CreateStaffAsync(CreateStaffRequest request,CancellationToken cancellationToken)
    {
       

        var email = request.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);
        var user = new ApplicationUser(email,request.FullName.Trim(),"en",DateTime.UtcNow);
        var createResult = await _userManager.CreateAsync(user,request.TemporaryPassword);
        var hasDuplicateError = createResult.Errors.Any( error => error.Code == "DuplicateEmail" || error.Code == "DuplicateUserName");
        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        return new CreateStaffResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = request.Role
        };
    }
}