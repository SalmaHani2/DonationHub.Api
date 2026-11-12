using DonationHub.Application.Admin.Options;
using DonationHub.Application.Auth.Options;
using DonationHub.Application.Donation.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DonationHub.Application.Common;


namespace DonationHub.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            {
                // Donation Options
                services
               .AddOptions<DonationOptions>()
               .BindConfiguration("Donation")
               .ValidateOnStart();



                

                services.AddOptions<JwtOptions>()
               .Bind(configuration.GetSection(JwtOptions.SectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();

                services.AddOptions<SeedAdminOptions>()
               .Bind(configuration.GetSection(SeedAdminOptions.SectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();



              
                return services;
            }
        }
    }
}
