using Azure.Messaging.ServiceBus;
using System.Collections.Concurrent;

namespace Mav.Infrastructure.ServiceBus.Implementations;

public class ServiceBusClientPool(ServiceBusClient serviceBusClient) : IServiceBusClientPool
{
    private readonly ServiceBusClient _serviceBusClient = serviceBusClient;
    private readonly ConcurrentDictionary<string, ServiceBusProcessor> _receivers = new();
    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

    public ServiceBusSender GetMessageSender(string queueOrTopicName)
    {
        if (_senders.TryGetValue(queueOrTopicName, out var value)) { return value; }

        value = _serviceBusClient.CreateSender(queueOrTopicName);

        _senders.TryAdd(queueOrTopicName, value);

        return value;
    }

    public ServiceBusProcessor GetMessageReceiver(string queueName)
    {
        if (_receivers.TryGetValue(queueName, out var value)) { return value; }

        value = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions { 
            AutoCompleteMessages = false
        });

        _receivers.TryAdd(queueName, value);

        return value;
    }    
}
