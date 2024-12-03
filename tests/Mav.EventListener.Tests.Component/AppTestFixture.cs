using Azure.Messaging.ServiceBus;
using Mav.Tests.Shared.ServiceBus;
using Moq;

namespace Mav.EventListener.Tests.Component;

public class AppTestFixture : IDisposable
{
    public readonly HttpClient HttpClient;
    public readonly AppWebApplicationFactory AppWebApplicationFactory;
    public readonly Mock<HttpMessageHandler> CustomerApiClientHttpMessageHandlerMock;

    public AppTestFixture()
    {
        AppWebApplicationFactory = new AppWebApplicationFactory();
        HttpClient = AppWebApplicationFactory.CreateClient();
        CustomerApiClientHttpMessageHandlerMock = AppWebApplicationFactory.CustomerApiClientHttpMessageHandlerMock;
    }

    public TestableServiceBusProcessor<ProcessMessageEventArgs>? GetTestableServiceBusProcessor(string queueName)
        => AppWebApplicationFactory.GetTestableServiceBusProcessor(queueName);

    public TestableServiceBusSender? GetTestableServiceBusSender(string queueOrTopicName)
        => AppWebApplicationFactory.GetTestableServiceBusSender(queueOrTopicName);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            AppWebApplicationFactory?.Dispose();
        }
    }
}
