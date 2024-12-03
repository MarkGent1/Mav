using Mav.Infrastructure.Requests.Interfaces;
using MediatR;

namespace Mav.Infrastructure.Requests;

public class RequestExecutor(IMediator mediator) : IRequestExecutor
{
    public async Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(query, cancellationToken);
    }
}
