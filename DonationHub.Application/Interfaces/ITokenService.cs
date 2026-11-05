namespace DonationHub.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId,string email,IList<string> roles);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}