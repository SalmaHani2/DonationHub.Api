using System.Security.Claims;
using DonationHub.Application.DTOs.Auth;
using DonationHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request,CancellationToken cancellationToken)
    {
        
            var result = await _authService.RegisterAsync(request,cancellationToken);

            return StatusCode(201, result);
    }

    // POST auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request,CancellationToken cancellationToken)
    {
       
            var result = await _authService.LoginAsync(request,cancellationToken);
            return Ok(result);
       
    }

    // POST auth/refresh
    [HttpPost("refresh")]
   
    public async Task<IActionResult> Refresh(RefreshRequest request,CancellationToken cancellationToken)
    {
            var result = await _authService.RefreshAsync(
                request,
                cancellationToken);

            return Ok(result);
      
    }

    // POST auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request,CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _authService.LogoutAsync(userId,request,cancellationToken);

        return NoContent();
    }

    // GET auth/me
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _authService.GetCurrentUserAsync(userId,cancellationToken);

        return Ok(user);
    }
}