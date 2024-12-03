using Azure.Messaging.ServiceBus;

namespace Mav.Tests.Shared.ServiceBus;

public class TestableProcessMessageEventArgs(ServiceBusReceivedMessage message) : ProcessMessageEventArgs(message, null, CancellationToken.None)
{
    public bool WasCompleted;
    public bool WasDeadLettered;
    public bool WasDeadAbandoned;
    public DateTime Created = DateTime.UtcNow;
    public string DeadLetterReason = string.Empty;
}
