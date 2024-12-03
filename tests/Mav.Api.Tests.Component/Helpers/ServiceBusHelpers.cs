using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Moq;

namespace Mav.Api.Tests.Component.Helpers;

public class ServiceBusHelpers
{
    internal static ServiceBusMessage VerifyMessagePublished(AppTestFixture appTestFixture, string topicOrQueueName, string subject)
    {
        var serviceBusSender = appTestFixture.GetTestableServiceBusSender(topicOrQueueName);
        serviceBusSender.Should().NotBeNull();

        var lastMessageDeliveryAttempt = serviceBusSender?.MessageDeliveryAttempts.LastOrDefault();
        lastMessageDeliveryAttempt.Should().NotBeNull();
        lastMessageDeliveryAttempt!.Subject.Should().Be(subject);

        return lastMessageDeliveryAttempt;
    }

    internal static void VerifyMessageWasCompleted(Mock<ProcessMessageEventArgs> processMessageEventArgsMock)
    {
        processMessageEventArgsMock.Verify(x => x.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    internal static void VerifyMessageWasDeadLetter(Mock<ProcessMessageEventArgs> processMessageEventArgsMock)
    {
        processMessageEventArgsMock.Verify(x => x.DeadLetterMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    internal static void VerifyMessageWasNotDeadLettered(Mock<ProcessMessageEventArgs> processMessageEventArgsMock)
    {
        processMessageEventArgsMock.Verify(x => x.DeadLetterMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }

    internal static void VerifyMessageWasAbandoned(Mock<ProcessMessageEventArgs> processMessageEventArgsMock)
    {
        processMessageEventArgsMock.Verify(x => x.AbandonMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    internal static void SetupDefaultProcessMessageEventArgsMock(Mock<ProcessMessageEventArgs> processMessageEventArgsMock)
    {
        processMessageEventArgsMock
            .Setup(x => x.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        processMessageEventArgsMock
            .Setup(x => x.DeadLetterMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        processMessageEventArgsMock
            .Setup(x => x.AbandonMessageAsync(It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
}
