using DonationHub.Application.DTOs.Admin;

namespace DonationHub.Application.Interfaces;

public interface IAdminService
{
    Task<CreateStaffResponse> CreateStaffAsync(CreateStaffRequest request,CancellationToken cancellationToken);
}