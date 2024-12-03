namespace Mav.Infrastructure.ServiceBus.Configuration;

public class ServiceBusConfiguration
{
    public string FullyQualifiedNamespace { get; init; } = null!;
    public string ServiceBusConnectionString { get; init; } = null!;
}
