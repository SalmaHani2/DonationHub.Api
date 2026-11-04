using DonationHub.Application.DTOs.Auth;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObject;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly AppDbContext _dbContext;
    private readonly JwtOptions _jwtOptions;

    public AuthService(UserManager<ApplicationUser> userManager,JwtTokenService jwtTokenService,AppDbContext dbContext,IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task RegisterAsync(RegisterRequest request,CancellationToken cancellationToken)
    {
        var user = new ApplicationUser(
            request.Email,
            request.FullName,
            request.PreferredLanguage,
            DateTime.UtcNow);

        await _userManager.CreateAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, "Donor");
    }


    public async Task<LoginResponse> LoginAsync( LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null ||
         !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Auth.InvalidCredentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Auth.AccountDeactivated");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtTokenService.GenerateToken(user, roles);

        var refreshToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var tokenHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

        var refreshTokenEntity = new RefreshToken
        {
            
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtOptions.RefreshTokenExpiryDays)
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }


    public async Task<LoginResponse> RefreshAsync(RefreshRequest request,CancellationToken cancellationToken)
    {
        var hash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));

        var oldToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

        if (oldToken == null ||
            oldToken.RevokedAt != null ||
            oldToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Auth.RefreshTokenInvalid");
        }

        var user = await _userManager.FindByIdAsync(oldToken.UserId.ToString());
        if (user == null ||
           !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Auth.InvalidCredentials");
        }
        if (user == null)
            throw new UnauthorizedAccessException("Auth.RefreshTokenInvalid");
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Auth.AccountDeactivated");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtTokenService.GenerateToken(user, roles);

        var newRefreshToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var newTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken))),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtOptions.RefreshTokenExpiryDays)
        };

        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.ReplacedByTokenId = newTokenEntity.Id;

        _dbContext.RefreshTokens.Add(newTokenEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }


    public async Task LogoutAsync(RefreshRequest request,CancellationToken cancellationToken)
    {
        var hash = Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(request.RefreshToken)));

        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == hash,
                cancellationToken);

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
