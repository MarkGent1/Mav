using Mav.Domain.Customers;
using Mav.Domain.Events;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mav.Infrastructure.Serlializers.Implementations;

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [typeof(JsonStringEnumConverter<CustomerType>)]
)]
[JsonSerializable(typeof(CustomerCreatedEvent))]
public partial class MessageFactorySerializerContext : JsonSerializerContext
{
}
