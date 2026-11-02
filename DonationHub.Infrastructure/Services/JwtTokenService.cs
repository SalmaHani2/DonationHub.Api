using DonationHub.Domain.ValueObject;
using DonationHub.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DonationHub.Infrastructure.Services;

public class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email", user.Email!)
        };

        foreach (var role in roles)
            claims.Add(new Claim("Roles", role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}