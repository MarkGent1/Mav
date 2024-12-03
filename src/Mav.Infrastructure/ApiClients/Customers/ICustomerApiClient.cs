using Mav.Infrastructure.ApiClients.Customers.Models;
using MediatR;
using Refit;

namespace Mav.Infrastructure.ApiClients.Customers;

public interface ICustomerApiClient
{
    [Post("/api/v1/customers")]
    Task<ApiResponse<Unit>> CreateAsync([Body] CreateCustomerRequest request);
}
