namespace Mav.Infrastructure.ApiClients.Policies;

public class ResiliencePolicy
{
    public int Retries { get; set; } = 0;
    public int MaxJitterMs { get; set; } = 0;
}
