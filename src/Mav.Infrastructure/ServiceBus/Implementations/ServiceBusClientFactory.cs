using Azure.Messaging.ServiceBus;

namespace Mav.Infrastructure.ServiceBus.Implementations;

public class ServiceBusClientFactory(IServiceBusClientPool? serviceBusClientPool) : IServiceBusClientFactory
{
    private readonly IServiceBusClientPool _serviceBusClientPool = serviceBusClientPool ?? throw new ArgumentNullException(nameof(serviceBusClientPool));

    public ServiceBusProcessor CreateReceiver(string queueName, Func<ProcessMessageEventArgs, Task>? messageHandler = null)
    {
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException(nameof(queueName));

        return _serviceBusClientPool.GetMessageReceiver(queueName);
    }

    public ServiceBusSender CreateSender(string queueOrTopicName)
    {
        if (string.IsNullOrWhiteSpace(queueOrTopicName)) throw new ArgumentException(nameof(queueOrTopicName));

        return _serviceBusClientPool.GetMessageSender(queueOrTopicName);
    }
}
