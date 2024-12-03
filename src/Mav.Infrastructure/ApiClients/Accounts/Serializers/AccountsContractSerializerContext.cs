using Mav.Domain.Accounts;
using Mav.Infrastructure.ApiClients.Accounts.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mav.Infrastructure.ApiClients.Accounts.Serializers;

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [typeof(JsonStringEnumConverter<AccountType>)]
)]
[JsonSerializable(typeof(AccountDetailsResponse))]
public partial class AccountsContractSerializerContext : JsonSerializerContext
{
}
