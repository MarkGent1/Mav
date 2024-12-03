using MediatR;

namespace Mav.Infrastructure.Requests.Interfaces;

public interface ICommand
{
}

public interface ICommand<T> : IRequest<T>, ICommand
{
}
