using DonationHub.Application.Options;
using Microsoft.Extensions.DependencyInjection;
namespace DonationHub.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
        {
            // Donation Options
                 services
                .AddOptions<DonationOptions>()
                .BindConfiguration("Donation")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
