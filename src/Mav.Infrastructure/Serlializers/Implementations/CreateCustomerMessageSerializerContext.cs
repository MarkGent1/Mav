using Mav.Domain.Customers;
using Mav.Domain.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mav.Infrastructure.Serlializers.Implementations;

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [typeof(JsonStringEnumConverter<CustomerType>)]
)]
[JsonSerializable(typeof(CreateCustomerMessage))]
public partial class CreateCustomerMessageSerializerContext : JsonSerializerContext
{
}
