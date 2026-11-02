
using DonationHub.Application.Interfaces;
using DonationHub.Application.Options;
using DonationHub.Domain.ValueObject;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWorkImplementation = DonationHub.Infrastructure.UnitOfWork.UnitOfWork;

namespace DonationHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            options.AddInterceptors(
                serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
        });

        // Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>();

        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWorkImplementation>();

        // Infrastructure Services
        services.AddScoped<JwtTokenService>();

        // Options
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SeedAdminOptions>()
            .Bind(configuration.GetSection(SeedAdminOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<DonationOptions>()
            .Bind(configuration.GetSection("Donation"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Admin Seeder
        services.AddScoped<AdminSeeder>();

        return services;
    }
}
