using Azure.Messaging.ServiceBus;
using Mav.Infrastructure.ApiClients.Customers;
using Mav.Tests.Shared;
using Mav.Tests.Shared.ServiceBus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Moq;
using Refit;

namespace Mav.EventListener.Tests.Component;

public class AppWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly Mock<HttpMessageHandler> CustomerApiClientHttpMessageHandlerMock = new();
    public Dictionary<string, TestableServiceBusProcessor<ProcessMessageEventArgs>> TestableServiceBusProcessors { get; } = [];
    public Dictionary<string, TestableServiceBusSender> TestableServiceBusSenders { get; } = [];
    public Mock<ServiceBusClient>? ServiceBusClientMock;

    private IHost? host;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("AccountApiConfiguration__BaseUrl", TestConstants.AccountApiBaseUrl);
        Environment.SetEnvironmentVariable("CustomerApiConfiguration__BaseUrl", TestConstants.CustomerApiBaseUrl);

        builder.ConfigureServices(services =>
        {
            services.AddRefitClient<ICustomerApiClient>()
                .ConfigurePrimaryHttpMessageHandler(() => CustomerApiClientHttpMessageHandlerMock.Object);

            RemoveService<IHealthCheckPublisher>(services);

            ConfigureServiceBus(services);
        });

        host = base.CreateHost(builder);

        return host;
    }

    public TestableServiceBusProcessor<ProcessMessageEventArgs>? GetTestableServiceBusProcessor(string queueName)
    {
        TestableServiceBusProcessors.TryGetValue(queueName, out var value);
        return value;
    }

    public TestableServiceBusSender? GetTestableServiceBusSender(string topicOrQueueName)
    {
        TestableServiceBusSenders.TryGetValue(topicOrQueueName, out var value);
        return value;
    }

    private void ConfigureServiceBus(IServiceCollection services)
    {
        ServiceBusClientMock = new Mock<ServiceBusClient>();
        ServiceBusClientMock
            .Setup(x => x.CreateProcessor(It.IsAny<string>(), It.IsAny<ServiceBusProcessorOptions>()))
            .Returns((string queueName, ServiceBusProcessorOptions _) =>
            {
                var testableProcessor = new TestableServiceBusProcessor<ProcessMessageEventArgs>();
                TestableServiceBusProcessors.Add(queueName, testableProcessor);
                return testableProcessor;
            });
        ServiceBusClientMock
            .Setup(x => x.CreateSender(It.IsAny<string>()))
            .Returns((string topicOrQueueName) =>
            {
                var testableSender = new TestableServiceBusSender();
                TestableServiceBusSenders.Add(topicOrQueueName, testableSender);
                return testableSender;
            });
        services.AddSingleton(ServiceBusClientMock.Object);
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var service = services.FirstOrDefault(x => x.ServiceType == typeof(T));
        if (service != null)
        {
            services.Remove(service);
        }
    }
}
