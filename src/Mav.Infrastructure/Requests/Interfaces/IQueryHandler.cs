using MediatR;

namespace Mav.Infrastructure.Requests.Interfaces;

public interface IQueryHandler<in TQuery, TResult> 
    : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}
