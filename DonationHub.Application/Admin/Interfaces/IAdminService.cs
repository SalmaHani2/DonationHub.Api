using DonationHub.Application.Admin.Admin;

namespace DonationHub.Application.Admin.Interfaces;

public interface IAdminService
{
    Task<CreateStaffResponse> CreateStaffAsync(CreateStaffRequest request,CancellationToken cancellationToken);
}