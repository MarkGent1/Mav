using Mav.Domain.Messages;
using Mav.EventListener.Processors;
using Mav.Infrastructure.ApiClients.IoC;
using Mav.Infrastructure.Commands.Customers;
using Mav.Infrastructure.Requests.IoC;
using Mav.Infrastructure.Serlializers;
using Mav.Infrastructure.Serlializers.Implementations;
using Mav.Infrastructure.ServiceBus;
using Mav.Infrastructure.ServiceBus.IoC;
using Mav.Infrastructure.ServiceBus.Processors;
using Mav.Infrastructure.Telemetry;
using System.Reflection;

namespace Mav.EventListener.Setup;

public static class ServiceRegistrations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        services.AddLogging();

        services.AddApplicationInsightsHostedService(config);

        services.AddHealthChecks();

        services.AddQueueListenerAsHostedService<CreateCustomerMessage>();

        services.AssServiceBusDependencies(config);

        services.AddCommandHandlers();

        services.AddApiClients(config);

        services.AddServiceBusReceivedMessageSerializers();

        services.AddServiceBusMessageProcessors();
    }

    private static void AddQueueListenerAsHostedService<T>(this IServiceCollection services)
    {
        services.AddHostedService<QueueListener<T>>()
            .AddSingleton<QueueListener<T>>();
    }

    public static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddRequestExecutor();

        Assembly[] assemblyNames = [typeof(CreateCustomerCommand).GetTypeInfo().Assembly];
        services.AddMediatR(p => p.RegisterServicesFromAssemblies(assemblyNames));
    }

    private static IServiceCollection AddServiceBusReceivedMessageSerializers(this  IServiceCollection services)
    {
        services.AddSingleton<IServiceBusReceivedMessageSerializer<CreateCustomerMessage>, CreateCustomerMessageSerializer>();

        return services;
    }

    private static IServiceCollection AddServiceBusMessageProcessors(this IServiceCollection services)
    {
        services.AddTransient<IMessageProcessor<CreateCustomerMessage>, CreateCustomerMessageProcessor>();

        return services;
    }
}
