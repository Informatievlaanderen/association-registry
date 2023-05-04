namespace AssociationRegistry.Test.Admin.Api.When_Validating_Initiator_Header;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Categories;

[IntegrationTest]
[Category("Middleware")]
public class Given_A_Http_Method : IAsyncLifetime
{
    private IHost _host = null!;

    public async Task InitializeAsync()
    {
        _host = await new HostBuilder().ConfigureWebHost(
                webBuilder =>
                {
                    webBuilder.UseTestServer()
                        .ConfigureServices(services => { services.AddSingleton(new ProblemDetailsHelper(null)); })
                        .Configure(app => { app.UseMiddleware<InitiatorHeaderMiddleware>(); });
                })
            .StartAsync();
    }

    [Theory]
    [InlineData("DELETE")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    public async Task Methods_Without_Initiator_Header_Return_400BadRequest(string methodAsString)
    {
        var httpMethod = new HttpMethod(methodAsString);

        var testClient = _host.GetTestClient();
        var response = await testClient.SendAsync(new HttpRequestMessage(httpMethod, "/"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("DELETE")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    [InlineData("OPTIONS")]
    [InlineData("GET")]
    [InlineData("HEAD")]
    public async Task Methods_With_Initiator_Header_Do_Not_Return_400BadRequest(string methodAsString)
    {
        var testClient = _host.GetTestClient();
        testClient.DefaultRequestHeaders.Add(WellknownHeaderNames.Initiator, "Koen");

        var httpMethod = new HttpMethod(methodAsString);
        var response = await testClient.SendAsync(new HttpRequestMessage(httpMethod, "/"));

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("OPTIONS")]
    [InlineData("GET")]
    [InlineData("HEAD")]
    public async Task Methods_Without_Initiator_Header_Do_Not_Return_400BadRequest(string methodAsString)
    {
        var testClient = _host.GetTestClient();

        var httpMethod = new HttpMethod(methodAsString);
        var response = await testClient.SendAsync(new HttpRequestMessage(httpMethod, "/"));

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    public async Task DisposeAsync()
        => _host.Dispose();
}
