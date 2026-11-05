using System.Security.Claims;
using System.Text;
using System.Net.Mail;
using DonationHub.Application.Interfaces;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Seed;
using DonationHub.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Database
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        // Identity
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        // JWT Options
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options =>!string.IsNullOrWhiteSpace(options.SecretKey) && Encoding.UTF8.GetByteCount(options.SecretKey) >= 32,"JWT SecretKey must contain at least 32 bytes.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer),"JWT Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),"JWT Audience is required.")
            .Validate(options => options.AccessTokenExpirationMinutes > 0,"Access token expiration must be greater than zero.")
            .Validate(options => options.RefreshTokenExpirationDays > 0,"Refresh token expiration must be greater than zero.")
            .ValidateOnStart();

        // Seed Admin Options
        services
            .AddOptions<SeedAdminOptions>()
            .Bind(configuration.GetSection(SeedAdminOptions.SectionName))
            .Validate(options =>!string.IsNullOrWhiteSpace(options.Email) &&MailAddress.TryCreate(options.Email, out _),"A valid SeedAdmin email is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.FullName), "SeedAdmin FullName is required.")
            .Validate( options => !string.IsNullOrWhiteSpace(options.Password),"SeedAdmin Password is required.")
            .ValidateOnStart();


        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();
        //services.AddScoped<ICategoriesService, CategoriesService>();
        services.AddScoped<AdminSeeder>();


        return services;
    }
}