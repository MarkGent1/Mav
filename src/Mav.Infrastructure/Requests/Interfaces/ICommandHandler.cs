using MediatR;

namespace Mav.Infrastructure.Requests.Interfaces;

public interface ICommandHandler<in TCommand, TResult> 
    : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
}
