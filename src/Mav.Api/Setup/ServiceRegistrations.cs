using Asp.Versioning;
using Mav.Domain.Events;
using Mav.Infrastructure.ApiClients.IoC;
using Mav.Infrastructure.Commands.Customers;
using Mav.Infrastructure.Requests.IoC;
using Mav.Infrastructure.ServiceBus.IoC;
using Mav.Infrastructure.ServiceBus.Publishers;
using Mav.Infrastructure.ServiceBus.Publishers.Implementations;
using Mav.Infrastructure.Telemetry;
using System.Reflection;

namespace Mav.Api.Setup
{
    public static class ServiceRegistrations
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;

            services.AddLogging();

            services.AddApplicationInsightsApi(config);

            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options => 
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddHealthChecks();

            services.AssServiceBusDependencies(config);

            services.AddCommandHandlers();

            services.AddApiClients(config);

            services.AddServiceBusMessagePublishers();
        }

        public static void AddCommandHandlers(this IServiceCollection services)
        {
            services.AddRequestExecutor();

            Assembly[] assemblyNames = [typeof(CreateCustomerCommand).GetTypeInfo().Assembly];
            services.AddMediatR(p => p.RegisterServicesFromAssemblies(assemblyNames));
        }

        public static IServiceCollection AddServiceBusMessagePublishers(this IServiceCollection services)
        {
            services.AddSingleton<IMessagePublisher<CustomerCreatedEvent>, CustomerCreatedEventPublisher>();

            return services;
        }
    }
}
