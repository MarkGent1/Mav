using Azure.Messaging.ServiceBus;
using Mav.Infrastructure;
using Moq;
using System.Text;
using System.Text.Json;

namespace Mav.Tests.Shared.ServiceBus;

public class ServiceBusMessageUtility
{
    public static ServiceBusReceivedMessage CreateServiceBusReceivedMessage<TMessage>(TMessage message, Guid lockToken, int deliveryCount)
    {
        var messageSerialized = JsonSerializer.Serialize(message, JsonDefaults.DefaultOptionsWithStringEnumConversion);
        var serviceBusMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(new BinaryData(Encoding.UTF8.GetBytes(messageSerialized)),
            lockTokenGuid: lockToken,
            sequenceNumber: 0,
            deliveryCount: deliveryCount);

        return serviceBusMessage;
    }

    public static Mock<ProcessMessageEventArgs> SetupProcessMessageEventArgsMock(ServiceBusReceivedMessage message, Mock<ServiceBusReceiver>? mockReceiver = null)
    {
        mockReceiver = SetupMockServiceBusReceiver(mockReceiver);

        return new Mock<ProcessMessageEventArgs>(MockBehavior.Default,
            message,
            mockReceiver!.Object,
            CancellationToken.None);
    }

    private static Mock<ServiceBusReceiver>? SetupMockServiceBusReceiver(Mock<ServiceBusReceiver>? mockReceiver)
    {
        if (mockReceiver == null)
        {
            mockReceiver = new();

            mockReceiver
                .Setup(receiver => receiver.CompleteMessageAsync(
                    It.IsAny<ServiceBusReceivedMessage>(), 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        return mockReceiver;
    }
}
