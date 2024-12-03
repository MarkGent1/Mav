using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Mav.Infrastructure.ApiClients.Policies;

public class HttpClientPolicyBuilder(ILogger<HttpClientPolicyBuilder> logger)
{
    public const int MaxSleepDurationSeconds = 100;

    public IAsyncPolicy<HttpResponseMessage> GetDefaultPolicy(ResiliencePolicy resiliencePolicy)
    {
        var jitterer = new Random();
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(resiliencePolicy.Retries, retryAttempt =>
                {
                    logger.LogInformation("Retry attempt: {retryAttempt} of {retries}.", retryAttempt, resiliencePolicy.Retries);

                    return TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), MaxSleepDurationSeconds)) + 
                        TimeSpan.FromMilliseconds(jitterer.Next(0, resiliencePolicy.MaxJitterMs));
                });
    }
}
