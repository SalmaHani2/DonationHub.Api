using DonationHub.Application.DTOs.Auth;

namespace DonationHub.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request,CancellationToken cancellationToken);

    Task<LoginResponse> LoginAsync(LoginRequest request,CancellationToken cancellationToken);

    Task<LoginResponse> RefreshAsync(RefreshRequest request,CancellationToken cancellationToken);

    Task LogoutAsync(Guid userId,RefreshRequest request,CancellationToken cancellationToken);

    Task<UserProfileResponse?> GetCurrentUserAsync(Guid userId,CancellationToken cancellationToken);
}