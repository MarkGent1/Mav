using Azure.Core;
using Azure.Identity;
using Mav.Infrastructure.ServiceBus.Configuration;
using Mav.Infrastructure.ServiceBus.Implementations;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Infrastructure.ServiceBus.IoC;

public static class ServiceBusRegistrations
{
    public static IServiceCollection AssServiceBusDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton(_ => configuration.GetSection("ServiceBusConfiguration").Get<ServiceBusConfiguration>()!)
            .AddAzureClient(configuration)
            .AddSingleton<IServiceBusClientPool, ServiceBusClientPool>()
            .AddSingleton<IServiceBusClientFactory, ServiceBusClientFactory>()
            .AddTransient<IMessageFactory, MessageFactory>();
    }

    private static IServiceCollection AddAzureClient(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusConfig = configuration.GetSection("ServiceBusConfiguration").Get<ServiceBusConfiguration>();

#if DEBUG
        services.AddAzureClients(cfg =>
        {
            cfg.AddServiceBusClient(serviceBusConfig!.ServiceBusConnectionString);
        });
#else
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(configuration.GetSection(nameof(ServiceBusConfiguration)));
            builder.UseCredential(new DefaultAzureCredential());
            services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
        });
#endif

        return services;
    }
}
