
using System.Text;
using System.Net.Mail;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Seed;
using DonationHub.Infrastructure.Services;
using DonationHub.Infrastructure.UnitOfWorks;
using DonationHub.Infrastructure.Repositories;
using DonationHub.Application.Admin.Interfaces;
using DonationHub.Application.Auth.Interfaces;
using DonationHub.Application.Tokens;
using DonationHub.Application.Auth.Options;
using DonationHub.Application.Admin.Options;
using DonationHub.Application.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using DonationHub.Application.Categories.Interfaces;

namespace DonationHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // TimeProvider
        services.AddSingleton(TimeProvider.System);

        // Soft Delete Interceptor
        services.AddScoped<SoftDeleteInterceptor>();

        // Database
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);

            options.AddInterceptors(
                serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
        });

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
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.SecretKey) &&
                Encoding.UTF8.GetByteCount(options.SecretKey) >= 32,
                "JWT SecretKey must contain at least 32 bytes.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer),
                "JWT Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),
                "JWT Audience is required.")
            .Validate(options => options.AccessTokenExpirationMinutes > 0,
                "Access token expiration must be greater than zero.")
            .Validate(options => options.RefreshTokenExpirationDays > 0,
                "Refresh token expiration must be greater than zero.")
            .ValidateOnStart();

        // JWT Authentication
        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey)
                    ),

                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Seed Admin Options
        services
            .AddOptions<SeedAdminOptions>()
            .Bind(configuration.GetSection(SeedAdminOptions.SectionName))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.Email) &&
                MailAddress.TryCreate(options.Email, out _),
                "A valid SeedAdmin email is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.FullName),
                "SeedAdmin FullName is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password),
                "SeedAdmin Password is required.")
            .ValidateOnStart();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();
         services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<AdminSeeder>();

        services.AddScoped<IUnitOfWorks, UnitOfWork>();

        return services;
    }
}
