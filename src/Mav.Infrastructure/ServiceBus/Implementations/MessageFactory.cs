using Azure.Messaging.ServiceBus;
using Mav.Domain.Events;
using Mav.Infrastructure.Serlializers.Implementations;
using System.Text;
using System.Text.Json;

namespace Mav.Infrastructure.ServiceBus.Implementations;

public class MessageFactory : IMessageFactory
{
    private const string EventId = "EventId";
    private const string EventTimeUtc = "EventTimeUtc";

    public ServiceBusMessage CreateWithDelay<TBody>(TBody body, TimeSpan enqueueDelay, IDictionary<string, object>? additionalUserProperties = null, string? subject = null)
    {
        return CreateMessage(body, enqueueDelay, additionalUserProperties, subject);
    }

    public ServiceBusMessage CreateMessage<TBody>(TBody body, IDictionary<string, object>? additionalUserProperties = null, string? subject = null)
    {
        return CreateMessage(body, TimeSpan.Zero, additionalUserProperties, subject);
    }

    private static ServiceBusMessage CreateMessage<TBody>(TBody body, TimeSpan enqueueDelay, IDictionary<string, object>? additionalUserProperties = null, string? subject = null)
    {
        return CreateMessage(Encoding.UTF8.GetBytes(SerializeToJson(body!)), enqueueDelay, additionalUserProperties, subject);
    }

    private static ServiceBusMessage CreateMessage(byte[] body, TimeSpan enqueueDelay, IDictionary<string, object>? additionalUserProperties, string? subject = null)
    {
        var dateTime = DateTime.UtcNow;

        var message = new ServiceBusMessage(body)
        {
            Subject = subject,
            ScheduledEnqueueTime = dateTime.AddSeconds(enqueueDelay.TotalSeconds)
        };

        message.ApplicationProperties.Add(EventTimeUtc, dateTime);
        message.ApplicationProperties.Add(EventId, Guid.NewGuid().ToString("N"));

        if (additionalUserProperties == null || !additionalUserProperties.Any()) 
        { 
            return message;
        }

        foreach (var (key, value) in additionalUserProperties)
        {
            message.ApplicationProperties.Add(key, value);
        }

        return message;
    }

    private static string SerializeToJson<TBody>(TBody value)
    {
        return typeof(TBody) switch
        {
            Type t when t == typeof(CustomerCreatedEvent) => JsonSerializer.Serialize(value, MessageFactorySerializerContext.Default.CustomerCreatedEvent),
            _ => JsonSerializer.Serialize(value, JsonDefaults.DefaultOptionsWithStringEnumConversion)
        };
    }
}