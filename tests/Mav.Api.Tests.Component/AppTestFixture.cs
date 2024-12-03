using Azure.Messaging.ServiceBus;
using Mav.Tests.Shared.ServiceBus;
using Moq;

namespace Mav.Api.Tests.Component;

public class AppTestFixture : IDisposable
{
    public readonly HttpClient HttpClient;
    public readonly AppWebApplicationFactory AppWebApplicationFactory;
    public readonly Mock<HttpMessageHandler> AccountApiClientHttpMessageHandlerMock;

    public AppTestFixture()
    {
        AppWebApplicationFactory = new AppWebApplicationFactory();
        HttpClient = AppWebApplicationFactory.CreateClient();
        AccountApiClientHttpMessageHandlerMock = AppWebApplicationFactory.AccountApiClientHttpMessageHandlerMock;
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
