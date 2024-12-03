using Mav.Infrastructure.ApiClients.Accounts.Models;
using Refit;

namespace Mav.Infrastructure.ApiClients.Accounts;

public interface IAccountApiClient
{
    [Get("/api/v1/accounts/{customerId}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<ApiResponse<AccountDetailsResponse>> GetAccountByCustomerIdAsync(string customerId);
}
