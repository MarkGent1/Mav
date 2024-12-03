using Mav.Infrastructure.ApiClients.Accounts;
using Mav.Infrastructure.ApiClients.Customers;
using Mav.Infrastructure.ApiClients.Policies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Mav.Infrastructure.ApiClients.IoC;

public static class ApiClientRegistrations
{
    public static void AddApiClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<HttpClientPolicyBuilder>();

        services.AddAccountClient(configuration);

        services.AddCustomerClient(configuration);
    }

    public static void RegisterRefitClient<T>(this IServiceCollection services,
        Func<ApiConfiguration> resolveConfig) where T : class
    {
        var config = resolveConfig();
        var contentSerializer = JsonDefaults.DefaultSystemTextJsonContentSerializer;
        var httpClientBuilder = services.AddRefitClient<T>(new RefitSettings(
            contentSerializer: contentSerializer))
            .ConfigureHttpClient((_, client) => 
            {
                client.BaseAddress = new Uri(config.BaseUrl!.TrimEnd('/'));
            });

        if (config.HasRetries())
        {
            httpClientBuilder.AddPolicyHandler(services.BuildServiceProvider().GetService<HttpClientPolicyBuilder>()!
                .GetDefaultPolicy(config.ResiliencePolicy));
        }
    }
}
