using Mav.Infrastructure.ApiClients.Customers.Configuration;
using Mav.Infrastructure.ApiClients.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Infrastructure.ApiClients.Customers;

public static class Registrations
{
    public static void AddCustomerClient(this IServiceCollection services, IConfiguration configuration)
    {
        var customerApiConfiguration = configuration.GetSection(nameof(CustomerApiConfiguration))
            .Get<CustomerApiConfiguration>()!;

        services.AddSingleton(customerApiConfiguration);

        services.RegisterRefitClient<ICustomerApiClient>(() => customerApiConfiguration);
    }
}
