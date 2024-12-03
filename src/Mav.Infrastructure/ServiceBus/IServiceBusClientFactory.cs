using Azure.Messaging.ServiceBus;

namespace Mav.Infrastructure.ServiceBus;

public interface IServiceBusClientFactory
{
    ServiceBusProcessor CreateReceiver(string queueName,
        Func<ProcessMessageEventArgs, Task>? messageHandler = null);

    ServiceBusSender CreateSender(string queueOrTopicName);
}
