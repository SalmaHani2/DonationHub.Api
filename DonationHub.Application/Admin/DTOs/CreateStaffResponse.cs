namespace DonationHub.Application.Admin.Admin;

public class CreateStaffResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}