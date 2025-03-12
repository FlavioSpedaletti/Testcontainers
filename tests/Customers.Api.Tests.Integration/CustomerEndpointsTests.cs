using System.Net;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Customers.Api.Database;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace Customers.Api.Tests.Integration;

public class CustomerEndpointsTests(ITestOutputHelper testOutputHelper) : IAsyncLifetime
{
    private HttpClient _client = null!;

    private readonly Faker<CustomerRequest> _customerGenerator =
        new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth.Date))
            .UseSeed(1000);

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("mydb")
            .WithUsername("workshop")
            .WithPassword("changeme")
            .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var waf = new WebApplicationFactory<IApiMarker>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(x =>
                {
                    x.ClearProviders();
                    x.SetMinimumLevel(LogLevel.Debug);
                    x.AddFilter(_ => true);

                    x.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(testOutputHelper));
                });

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.RemoveAll<AppDbContext>();

                    services.AddDbContext<AppDbContext>(x =>
                        x.UseNpgsql(_dbContainer.GetConnectionString()));
                });
            });

        _client = waf.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldCreateCustomer_WhenDetailsAreValid()
    {
        // Arrange
        var request = _customerGenerator.Generate();

        var expectedResponse = new CustomerResponse
        {
            Email = request.Email,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth
        };

        // Act
        var response = await _client.PostAsJsonAsync("customers", request);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"http://localhost/customers/{customerResponse!.Id}");
        customerResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        customerResponse.Id.Should().NotBeEmpty();
    }

    //public async Task DisposeAsync()
    //{
    //    await _dbContainer.StopAsync();
    //}

    public Task DisposeAsync()
    {
        return _dbContainer.DisposeAsync().AsTask();
    }
}
