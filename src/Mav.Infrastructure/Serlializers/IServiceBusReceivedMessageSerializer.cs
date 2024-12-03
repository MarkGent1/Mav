using Azure.Messaging.ServiceBus;

namespace Mav.Infrastructure.Serlializers;

public interface IServiceBusReceivedMessageSerializer<out T>
{
    T? Deserialize(ServiceBusReceivedMessage message);
}
