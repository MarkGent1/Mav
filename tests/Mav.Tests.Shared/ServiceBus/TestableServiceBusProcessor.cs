using Azure.Messaging.ServiceBus;
using Mav.Infrastructure;
using Mav.Infrastructure.Context;
using System.Text.Json;

namespace Mav.Tests.Shared.ServiceBus;

public class TestableServiceBusProcessor<T> : ServiceBusProcessor where T : class
{
    public List<TestableProcessMessageEventArgs> MessageDeliveryAttempts = [];

    public async Task SendMessageWithRetries(T request, int maxDeliveryCount = 10)
    {
        for (var attempt = 1; attempt <= maxDeliveryCount; attempt++)
        {
            if (attempt > 1)
            {
                var previousAttempt = MessageDeliveryAttempts.Last();
                if (previousAttempt.WasDeadLettered || previousAttempt.WasCompleted)
                    return;
            }

            await SendMessage(request, attempt);
        }
    }

    public async Task SendMessage(T request, int deliveryCount = 1)
    {
        var args = CreateMessageArgs(request, deliveryCount);
        MessageDeliveryAttempts.Add((TestableProcessMessageEventArgs)args);
        await base.OnProcessMessageAsync(args);
    }

    public async Task SendMessage(string json)
    {
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(body: BinaryData.FromString(json));

        var args = new TestableProcessMessageEventArgs(message);

        MessageDeliveryAttempts.Add(args);

        await base.OnProcessMessageAsync(args);
    }

    public override Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public static ProcessMessageEventArgs CreateMessageArgs(T payload, int deliveryCount = 1)
    {
        var payloadJson = JsonSerializer.Serialize(payload, JsonDefaults.DefaultOptionsWithStringEnumConversion);

        var correlationId = Guid.NewGuid().ToString();
        var snapshot = new LogicalExecutionContextSnapshot
        {
            Context =
            [
                new("CorrelationStack", correlationId)    
            ]
        };

        var applicationProperties = new Dictionary<string, object>()
        {
            { "logical-execution-context", Newtonsoft.Json.JsonConvert.SerializeObject(snapshot) }
        };

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(payloadJson),
            correlationId: correlationId,
            properties: applicationProperties,
            deliveryCount: deliveryCount);

        var args = new TestableProcessMessageEventArgs(message);

        return args;
    }
}
