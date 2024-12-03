using Azure.Messaging.ServiceBus;

namespace Mav.Infrastructure.ServiceBus;

public interface IMessageFactory
{
    ServiceBusMessage CreateWithDelay<TBody>(TBody body,
        TimeSpan enqueueDelay,
        IDictionary<string, object>? additionalUserProperties = null,
        string? subject = null);

    ServiceBusMessage CreateMessage<TBody>(TBody body,
        IDictionary<string, object>? additionalUserProperties = null,
        string? subject = null);
}
