using MediatR;

namespace Mav.Infrastructure.Requests.Interfaces;

public interface IQuery<T> : IRequest<T>
{
}
