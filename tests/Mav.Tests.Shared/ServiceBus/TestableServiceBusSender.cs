using Azure.Messaging.ServiceBus;

namespace Mav.Tests.Shared.ServiceBus;

public class TestableServiceBusSender : ServiceBusSender
{
    public List<ServiceBusMessage> MessageDeliveryAttempts = [];

    public override async Task SendMessageAsync(ServiceBusMessage message, CancellationToken cancellationToken)
    {
        MessageDeliveryAttempts.Add(message);
        await Task.CompletedTask;
    }
}
