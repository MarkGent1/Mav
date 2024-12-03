using Asp.Versioning;
using Mav.Infrastructure.ApiClients.Customers.Models;
using Mav.Infrastructure.Commands.Customers;
using Mav.Infrastructure.Requests.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mav.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public class CustomersController(IRequestExecutor requestExecutor) : ControllerBase
    {
        private readonly IRequestExecutor _requestExecutor = requestExecutor;

        [HttpPost]
        public async Task<Unit> CreateAsync([FromBody] CreateCustomerRequest createCustomerRequest, CancellationToken cancellationToken)
        {
            var command = new CreateCustomerCommand(createCustomerRequest.CustomerId);

            return await _requestExecutor.ExecuteCommand(command, cancellationToken);
        }
    }
}
