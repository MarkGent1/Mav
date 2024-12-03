using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mav.Infrastructure.ApiClients.Accounts.Configuration;
using Mav.Infrastructure.ApiClients.IoC;

namespace Mav.Infrastructure.ApiClients.Accounts;

public static class Registrations
{
    public static void AddAccountClient(this IServiceCollection services, IConfiguration configuration)
    {
        var accountApiConfiguration = configuration.GetSection(nameof(AccountApiConfiguration))
            .Get<AccountApiConfiguration>()!;

        services.AddSingleton(accountApiConfiguration);

        services.RegisterRefitClient<IAccountApiClient>(() => accountApiConfiguration);
    }
}
