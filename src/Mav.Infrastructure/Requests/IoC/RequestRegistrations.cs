using Mav.Infrastructure.Requests.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Infrastructure.Requests.IoC;

public static class RequestRegistrations
{
    public static void AddRequestExecutor(this IServiceCollection services)
    {
        services.AddTransient<IRequestExecutor, RequestExecutor>();
    }
}
