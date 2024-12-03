using Azure.Messaging.ServiceBus;
using Mav.Domain.Messages;
using System.Text.Json;

namespace Mav.Infrastructure.Serlializers.Implementations;

public class CreateCustomerMessageSerializer : IServiceBusReceivedMessageSerializer<CreateCustomerMessage>
{
    public CreateCustomerMessage? Deserialize(ServiceBusReceivedMessage message)
    {
        var messageBody = JsonSerializer.Deserialize<CreateCustomerMessage>(message.Body, CreateCustomerMessageSerializerContext.Default.CreateCustomerMessage);
        return messageBody;
    }
}
