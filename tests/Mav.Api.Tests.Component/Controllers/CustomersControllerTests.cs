using AutoFixture;
using Mav.Infrastructure.ApiClients.Accounts.Models;
using Mav.Infrastructure.ApiClients.Customers.Models;
using Mav.Tests.Shared;
using Mav.Tests.Shared.Builders;
using Mav.Tests.Shared.WebApi;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace Mav.Api.Tests.Component.Controllers;

public class CustomersControllerTests
{
    private readonly AppTestFixture _appTestFixture;

    private readonly Fixture _fixture;

    private const string GetAccountByCustomerIdEndpoint = $"{TestConstants.AccountApiBaseUrl}{TestConstants.GetAccountByCustomerIdEndpoint}";

#pragma warning disable xUnit1041 // Fixture arguments to test classes must have fixture sources
    public CustomersControllerTests(AppTestFixture appTestFixture)
#pragma warning restore xUnit1041 // Fixture arguments to test classes must have fixture sources
    {
        _appTestFixture = appTestFixture;
        _appTestFixture.AccountApiClientHttpMessageHandlerMock.Reset();

        _fixture = new Fixture();
        _fixture.Customizations.Add(new CustomContextDataSpecimenBuilder());
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GivenCreateAsync_ShouldComplete()
    {
        // Arrange
        var createCustomerRequest = new CreateCustomerRequest
        {
            CustomerId = Guid.NewGuid().ToString()
        };

        var requestContent = WebApiUtility.GetRequestContent(createCustomerRequest);

        var getAccountByCustomerIdEndpoint = GetAccountByCustomerIdEndpointUri(createCustomerRequest.CustomerId);

        var accountDetailsResponse = new AccountDetailsResponse
        {
            AccountId = Guid.NewGuid().ToString()
        };

        SetupGetAccountByCustomerIdRequest(getAccountByCustomerIdEndpoint, HttpStatusCode.OK, WebApiUtility.CreateResponseContent(accountDetailsResponse));

        // Act
        var response = await _appTestFixture.HttpClient.PostAsync("api/v1/customers", requestContent);

        // Assert
        response.EnsureSuccessStatusCode();
        VerifyAccountApiClientEndpointCalled(getAccountByCustomerIdEndpoint, Times.Once());
    }

    private static string GetAccountByCustomerIdEndpointUri(string id) => GetAccountByCustomerIdEndpoint.Replace("{customerId}", id);

    private void VerifyAccountApiClientEndpointCalled(string requestUrl, Times times)
    {
        _appTestFixture.AccountApiClientHttpMessageHandlerMock.VerifyRequest(HttpMethod.Get, requestUrl, times);
    }

    private void SetupGetAccountByCustomerIdRequest(string uri, HttpStatusCode httpStatusCode, StringContent httpResponseMessage)
    {
        _appTestFixture.AccountApiClientHttpMessageHandlerMock.SetupRequest(HttpMethod.Get, uri)
            .ReturnsResponse(httpStatusCode, httpResponseMessage);
    }
}
