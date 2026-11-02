using DonationHub.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using static DonationHub.Application.DTOs.Auth.RegisterRequest;

namespace DonationHub.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request,CancellationToken cancellationToken);
        Task<LoginResponse> LoginAsync(LoginRequest request,CancellationToken cancellationToken);
        Task<LoginResponse> RefreshAsync(RefreshRequest request,CancellationToken cancellationToken);
        Task LogoutAsync(RefreshRequest request,CancellationToken cancellationToken);
    }
}
