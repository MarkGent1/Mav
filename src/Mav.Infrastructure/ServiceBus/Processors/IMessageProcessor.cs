namespace Mav.Infrastructure.ServiceBus.Processors;

public interface IMessageProcessor<in T>
{
    string QueueName { get; }

    Task ProcessMessageAsync(T? message, CancellationToken cancellationToken = default);
}
