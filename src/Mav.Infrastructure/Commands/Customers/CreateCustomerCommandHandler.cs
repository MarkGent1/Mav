using Mav.Infrastructure.Exceptions;
using Mav.Infrastructure.Queries.Accounts;
using Mav.Infrastructure.Requests.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mav.Infrastructure.Commands.Customers;

public class CreateCustomerCommandHandler(IRequestExecutor requestExecutor,
    ILogger<CreateCustomerCommandHandler> logger) : ICommandHandler<CreateCustomerCommand, Unit>
{
    private readonly IRequestExecutor _requestExecutor = requestExecutor;
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;

    public async Task<Unit> Handle(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CreateCustomerCommandHandler processing CreateCustomerCommand for {CustomerId}", command.CustomerId);

        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<Unit>(cancellationToken);
        }

        var getAccountByCustomeQueryTask = _requestExecutor.ExecuteQuery(new GetAccountByCustomerQuery(command.CustomerId), cancellationToken);

        await Task.WhenAll(getAccountByCustomeQueryTask);

        var accountDetailsResponse = getAccountByCustomeQueryTask.Result;

        if (accountDetailsResponse == null)
            throw new NonRetryableException($"Account is null for CustomerId: {command.CustomerId}");

        // DO OTHER WORK HERE TO CREATE CUSTOMER

        return await Task.FromResult(Unit.Value);
    }
}
