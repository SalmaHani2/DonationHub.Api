using DonationHub.Application.DTOs.Auth;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.ValueObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
   private readonly IAuthService _authService;

    //public AuthController(IAuthService authService)
    //{
      //  _authService = authService;
    //}
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(IAuthService authService,UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.RegisterAsync(request, cancellationToken);

            return StatusCode(201, new
            {
                message = "User registered successfully."
            });
        }
        catch (InvalidOperationException ex)
            when (ex.Message == "Auth.EmailAlreadyExists")
        {
            return Conflict(new { code = "Auth.EmailAlreadyExists" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(
                request, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
            when (ex.Message == "Auth.AccountDeactivated")
        {
            return StatusCode(403, new
            {
                code = "Auth.AccountDeactivated"
            });
        }
        catch (UnauthorizedAccessException ex)
         when (ex.Message == "Auth.InvalidCredentials")
        {
            return Unauthorized(new
            {
                code = "Auth.InvalidCredentials"
            });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RefreshAsync(
                request, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
            when (ex.Message == "Auth.AccountDeactivated")
        {
            return StatusCode(403, new
            {
                code = "Auth.AccountDeactivated"
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                code = "Auth.RefreshTokenInvalid"
            });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request,CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request, cancellationToken);

        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Unauthorized();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.PreferredLanguage
        });
    }
}