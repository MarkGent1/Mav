using AutoFixture;
using Azure.Messaging.ServiceBus;
using Mav.Domain.Messages;
using Mav.EventListener.Tests.Component.Helpers;
using Mav.Infrastructure.ServiceBus;
using Mav.Tests.Shared;
using Mav.Tests.Shared.Builders;
using Mav.Tests.Shared.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace Mav.EventListener.Tests.Component.Processors;

public class CreateCustomerMessageProcessorTests : IClassFixture<AppTestFixture>
{
    private readonly AppTestFixture _appTestFixture;

    private readonly Fixture _fixture;

    private const string CreateCustomerEndpoint = $"{TestConstants.CustomerApiBaseUrl}{TestConstants.CreateCustomerEndpoint}";

    public CreateCustomerMessageProcessorTests(AppTestFixture appTestFixture)
    {
        _appTestFixture = appTestFixture;
        _appTestFixture.CustomerApiClientHttpMessageHandlerMock.Reset();

        _fixture = new Fixture();
        _fixture.Customizations.Add(new CustomContextDataSpecimenBuilder());
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GivenCreateCustomerMessage_ShouldComplete()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var createCustomerMessage = GetCreateCustomerMessage(customerId.ToString());
        var processMessageEventsArgsMock = GetMessageArgs(createCustomerMessage);
        ServiceBusHelpers.SetupDefaultProcessMessageEventArgsMock(processMessageEventsArgsMock);

        SetupCreateCustomerRequest(CreateCustomerEndpoint, HttpStatusCode.OK);

        // Act
        await ExecuteTest(processMessageEventsArgsMock);

        // Assert
        VerifyCustomerApiClientEndpointCalled(CreateCustomerEndpoint, Times.Once());
        ServiceBusHelpers.VerifyMessageWasCompleted(processMessageEventsArgsMock);
    }

    private static CreateCustomerMessage GetCreateCustomerMessage(string customerId) => new()
    {
        CustomerId = customerId
    };

    private static Mock<ProcessMessageEventArgs> GetMessageArgs(CreateCustomerMessage createCustomerMessage)
    {
        var message = ServiceBusMessageUtility.CreateServiceBusReceivedMessage(createCustomerMessage, Guid.NewGuid(), 1);
        var processMessageEventArgsMock = ServiceBusMessageUtility.SetupProcessMessageEventArgsMock(message);
        return processMessageEventArgsMock;
    }

    private async Task ExecuteTest(Mock<ProcessMessageEventArgs> processMessageEventArgsMock)
    {
        using var scope = _appTestFixture.AppWebApplicationFactory.Server.Services.CreateScope();
        var queueListener = scope.ServiceProvider.GetRequiredService<QueueListener<CreateCustomerMessage>>();

        await ReceiveMessageAsync();
        return;

        async Task ReceiveMessageAsync() => await queueListener.ReceiveMessageAsync(processMessageEventArgsMock.Object);
    }

    private void VerifyCustomerApiClientEndpointCalled(string requestUrl, Times times)
    {
        _appTestFixture.CustomerApiClientHttpMessageHandlerMock.VerifyRequest(HttpMethod.Post, requestUrl, times);
    }

    private void SetupCreateCustomerRequest(string uri, HttpStatusCode httpStatusCode)
    {
        _appTestFixture.CustomerApiClientHttpMessageHandlerMock.SetupRequest(HttpMethod.Post, uri)
            .ReturnsResponse(httpStatusCode);
    }
}
