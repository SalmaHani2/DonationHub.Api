using DonationHub.Application.DTOs.Auth;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(UserManager<ApplicationUser> userManager,AppDbContext context,ITokenService tokenService,IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    // IDN-01: Register
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request,CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var preferredLanguage =
            request.PreferredLanguage.Trim().ToLowerInvariant();

        var user = new ApplicationUser(email,request.FullName.Trim(),preferredLanguage,DateTime.UtcNow);

        if (user != null)
        {
            throw new InvalidOperationException("Auth.EmailAlreadyExists");
        }
        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            PreferredLanguage = user.PreferredLanguage
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request,CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null ||!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException(
                "Auth.InvalidCredentials");
        }
        if (!user.IsActive)
        {
            throw new InvalidOperationException(
                "Auth.AccountDeactivated");
        }

        //  Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        //  Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email!,
            roles);

        var refreshToken = _tokenService.GenerateRefreshToken();

        // Hash refresh token
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        //  Create refresh token entity
        var token = new RefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));

        //  Save refresh token
        _context.RefreshTokens.Add(token);

        await _context.SaveChangesAsync(cancellationToken);

        // Return response
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        var oldToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (oldToken == null ||
            oldToken.RevokedAt != null ||
            oldToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Auth.RefreshTokenInvalid");
        }

        var user = await _userManager.FindByIdAsync(oldToken.UserId.ToString());

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Auth.AccountDeactivated");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id, user.Email!, roles);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newTokenEntity = new RefreshToken(
            user.Id,
            _tokenService.HashRefreshToken(newRefreshToken),
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));

        // Revoke the old token and link it to the new token.
        oldToken.Revoke(newTokenEntity.Id);

        _context.RefreshTokens.Add(newTokenEntity);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(Guid userId,RefreshRequest request,CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        var refreshToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(token =>token.UserId == userId && token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<UserProfileResponse?> GetCurrentUserAsync(Guid userId,CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileResponse
        {  
            Email = user.Email!,
            FullName = user.FullName,
            PreferredLanguage = user.PreferredLanguage,
            Roles = roles
        };
    }


    
}