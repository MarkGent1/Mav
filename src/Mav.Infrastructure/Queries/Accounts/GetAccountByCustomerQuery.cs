using Mav.Infrastructure.ApiClients.Accounts.Models;
using Mav.Infrastructure.Requests.Interfaces;

namespace Mav.Infrastructure.Queries.Accounts;

public class GetAccountByCustomerQuery(string customerId) : IQuery<AccountDetailsResponse?>
{
    public string CustomerId { get; set; } = customerId;
}
