using Azure.Messaging.ServiceBus;

namespace Mav.Infrastructure.ServiceBus;

public interface IServiceBusClientPool
{
    ServiceBusProcessor GetMessageReceiver(string queueName);

    ServiceBusSender GetMessageSender(string queueOrTopicName);
}
