using DonationHub.Application.DTOs.Admin;
using DonationHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin,CampaignManager")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // POST admin/staff
    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff(CreateStaffRequest request,CancellationToken cancellationToken)
    {
        var result = await _adminService.CreateStaffAsync(request,cancellationToken);

        return StatusCode(201, result);
    }
}