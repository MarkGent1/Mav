using Mav.Infrastructure.ApiClients.Policies;

namespace Mav.Infrastructure.ApiClients;

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = default!;
    public string SubscriptionKey { get; set; } = default!;

    public ResiliencePolicy ResiliencePolicy { get; set; } = new();
    public bool HasRetries() => ResiliencePolicy.Retries > 0;
}
