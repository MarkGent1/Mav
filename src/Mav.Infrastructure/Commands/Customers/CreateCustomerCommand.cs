using Mav.Infrastructure.Requests.Interfaces;
using MediatR;

namespace Mav.Infrastructure.Commands.Customers;

public class CreateCustomerCommand(string customerId) : ICommand<Unit>
{
    public string CustomerId { get; set; } = customerId;
}
