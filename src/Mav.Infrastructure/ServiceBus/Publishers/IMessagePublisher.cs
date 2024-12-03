namespace Mav.Infrastructure.ServiceBus.Publishers;

public interface IMessagePublisher<in T>
{
    Task PublishAsync(T? message, CancellationToken cancellationToken = default);
}
