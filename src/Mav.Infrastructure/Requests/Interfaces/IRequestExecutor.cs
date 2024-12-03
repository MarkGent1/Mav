namespace Mav.Infrastructure.Requests.Interfaces;

public interface IRequestExecutor
{
    Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
