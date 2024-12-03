using Mav.Infrastructure.ApiClients.Accounts;
using Mav.Infrastructure.ApiClients.Accounts.Models;
using Mav.Infrastructure.ApiClients.Accounts.Serializers;
using Mav.Infrastructure.Exceptions;
using Mav.Infrastructure.Requests.Interfaces;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net;
using System.Text.Json;

namespace Mav.Infrastructure.Queries.Accounts;

public partial class GetAccountByCustomerQueryHandler(IAccountApiClient accountApiClient,
    ILogger<GetAccountByCustomerQueryHandler> logger) : IQueryHandler<GetAccountByCustomerQuery, AccountDetailsResponse?>
{
    private readonly IAccountApiClient _accountApiClient = accountApiClient;
    private readonly ILogger<GetAccountByCustomerQueryHandler> _logger = logger;

    [LoggerMessage(Level = LogLevel.Information, Message = "Account details: {accountId} > {serializedResult}")]
    static partial void LogAccountDetails(ILogger<GetAccountByCustomerQueryHandler> logger, string accountId, string serializedResult);

    public async Task<AccountDetailsResponse?> Handle(GetAccountByCustomerQuery request, CancellationToken cancellationToken)
    {
        AccountDetailsResponse? result = null;

        try
        {
            using var apiResponse = await _accountApiClient.GetAccountByCustomerIdAsync(request.CustomerId);

            if (!apiResponse.IsSuccessStatusCode)
                LogApiExceptionAndThrow(apiResponse.Error, apiResponse.StatusCode);

            if (apiResponse.Content == null)
                throw new NonRetryableException($"GetAccountByCustomerIdAsync response content is null");

            result = apiResponse.Content;

            var serializedResult = JsonSerializer.Serialize(result, AccountsContractSerializerContext.Default.AccountDetailsResponse);

            LogAccountDetails(_logger, result.AccountId, serializedResult);
        }
        catch (ApiException ex)
        {
            LogApiExceptionAndThrow(ex, ex.StatusCode);
        }

        return result;
    }

    private void LogApiExceptionAndThrow(ApiException? ex, HttpStatusCode statusCode)
    {
        if (ex == null) throw new NonRetryableException($"GetAccountByCustomerIdAsync execution failed with StatusCode (unknown ApiException): {statusCode}");

        _logger.LogInformation("GetAccountByCustomerIdAsync endpoint execution failed with StatusCode: {statusCode}, ReasonPhrase: {reasonPhrase}, Content: {content}",
            ex.StatusCode.ToString(),
            ex.ReasonPhrase ?? string.Empty,
            ex.HasContent ? ex.Content : string.Empty);

        if ((int)statusCode >= 500)
            throw new RetryableException($"GetAccountByCustomerIdAsync execution failed with StatusCode: {statusCode}", ex);

        throw new NonRetryableException($"GetAccountByCustomerIdAsync execution failed with StatusCode: {statusCode}", ex);
    }
}
