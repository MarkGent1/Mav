using Mav.Domain.Messages;
using Mav.Infrastructure.ApiClients.Customers;
using Mav.Infrastructure.ApiClients.Customers.Models;
using Mav.Infrastructure.ServiceBus;
using Mav.Infrastructure.ServiceBus.Processors;

namespace Mav.EventListener.Processors;

public partial class CreateCustomerMessageProcessor(ICustomerApiClient customerApiClient,
    ILogger<CreateCustomerMessageProcessor> logger) : IMessageProcessor<CreateCustomerMessage>
{
    private readonly ICustomerApiClient _customerApiClient = customerApiClient;
    private readonly ILogger<CreateCustomerMessageProcessor> _logger = logger;

    public string QueueName => ServiceBusQueues.CustomersQueueName;

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing CreateCustomerMessage -> CustomerId:{customerId}")]
    static partial void LogCreateCustomerInformation(ILogger<CreateCustomerMessageProcessor> logger, string customerId);

    public async Task ProcessMessageAsync(CreateCustomerMessage? message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        LogCreateCustomerInformation(_logger, message.CustomerId ?? string.Empty);

        var request = new CreateCustomerRequest
        {
            CustomerId = message.CustomerId ?? string.Empty
        };

        await _customerApiClient.CreateAsync(request);
    }
}
