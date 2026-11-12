
using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Auth.Interfaces;
using DonationHub.Application.Auth.Options;
using DonationHub.Application.Tokens;
using DonationHub.Application.Common;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IUnitOfWorks _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        IUnitOfWorks unitOfWork,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var preferredLanguage =
            request.PreferredLanguage.Trim().ToLowerInvariant();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "Auth.EmailAlreadyExists");
        }

        var user = new ApplicationUser(
            email,
            request.FullName.Trim(),
            preferredLanguage,
            DateTime.UtcNow);

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            PreferredLanguage = user.PreferredLanguage
        };
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null ||
            !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException(
                "Auth.InvalidCredentials");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException(
                "Auth.AccountDeactivated");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email!,
            roles);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenHash =
            _tokenService.HashRefreshToken(refreshToken);

        var token = new RefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(
                _jwtOptions.RefreshTokenExpirationDays));

        await _unitOfWork.Repository<RefreshToken>()
            .AddAsync(token, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResponse> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _tokenService.HashRefreshToken(request.RefreshToken);

        var oldToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

        if (oldToken == null ||
            oldToken.RevokedAt != null ||
            oldToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException(
                "Auth.RefreshTokenInvalid");
        }

        var user = await _userManager.FindByIdAsync(
            oldToken.UserId.ToString());

        if (user == null)
        {
            throw new UnauthorizedAccessException(
                "Auth.RefreshTokenInvalid");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException(
                "Auth.AccountDeactivated");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email!,
            roles);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newTokenEntity = new RefreshToken(
            user.Id,
            _tokenService.HashRefreshToken(newRefreshToken),
            DateTime.UtcNow.AddDays(
                _jwtOptions.RefreshTokenExpirationDays));

        oldToken.Revoke(
            newTokenEntity.Id,
            DateTime.UtcNow);

        await _unitOfWork.Repository<RefreshToken>()
            .AddAsync(newTokenEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(
        Guid userId,
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _tokenService.HashRefreshToken(request.RefreshToken);

        var refreshToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(
                token =>
                    token.UserId == userId &&
                    token.TokenHash == tokenHash,
                cancellationToken);

        if (refreshToken == null ||
            refreshToken.RevokedAt != null)
        {
            return;
        }

        refreshToken.Revoke(
            null,
            DateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserProfileResponse?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user == null)
        {
            return null;
        }

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
