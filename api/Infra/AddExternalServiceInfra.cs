using api.Services;
using Microsoft.Extensions.Options;

namespace api.Infra
{
    public static class AddExternalServiceInfra
    {

        public static IServiceCollection AddExternalServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ExternalServices>(configuration.GetSection("ExternalServices"));

            //Add service Transient scope by default
            services.AddHttpClient<IAccountService, AccountService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ExternalServices>>().Value;
                client.BaseAddress = new Uri(options.AccountUrl);
            });
            services.AddHttpClient<IExchangeRateService, ExchangeRateService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ExternalServices>>().Value;
                client.BaseAddress = new Uri(options.ExchangeRateUrl);
            });

            return services;
        }
    }
}
