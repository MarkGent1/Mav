using Azure.Messaging.ServiceBus;
using Mav.Domain.Events;
using Mav.Infrastructure.ServiceBus.Exceptions;
using System.Runtime.Serialization;

namespace Mav.Infrastructure.ServiceBus.Publishers.Implementations;

public class CustomerCreatedEventPublisher(IMessageFactory messageFactory,
    IServiceBusClientFactory serviceBusClientFactory) : IMessagePublisher<CustomerCreatedEvent>
{
    private readonly IMessageFactory _messageFactory = messageFactory;
    private readonly IServiceBusClientFactory _serviceBusClientFactory = serviceBusClientFactory;

    public async Task PublishAsync(CustomerCreatedEvent? customerCreatedEvent, CancellationToken cancellationToken = default)
    {
        if (customerCreatedEvent == null) return;

        try
        {
            var senderClient = _serviceBusClientFactory.CreateSender(ServiceBusTopics.CustomersQueueName);
            var message = _messageFactory.CreateMessage(customerCreatedEvent);
            await senderClient.SendMessageAsync(message, cancellationToken);
        }
        catch (SerializationException sx)
        {
            throw new PublishFailedException($"Failed to publish event on {ServiceBusTopics.CustomersQueueName} due to serialization exception.", false, sx);
        }
        catch (ServiceBusException ex)
        {
            throw new PublishFailedException($"Failed to publish event on {ServiceBusTopics.CustomersQueueName}.", ex.IsTransient, ex);
        }
    }
}
