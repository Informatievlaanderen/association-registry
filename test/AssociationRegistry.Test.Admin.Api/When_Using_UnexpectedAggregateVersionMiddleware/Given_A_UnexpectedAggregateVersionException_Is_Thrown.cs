namespace AssociationRegistry.Test.Admin.Api.When_Using_UnexpectedAggregateVersionMiddleware;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using EventStore;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Middleware")]
public class Given_A_UnexpectedAggregateVersionException_Is_Thrown : IAsyncLifetime
{
    private IHost _host = null!;
    private const string Route = "/";

    public async Task InitializeAsync()
    {
        _host = await new HostBuilder().ConfigureWebHost(
                webBuilder =>
                {
                    webBuilder.UseTestServer()
                        .ConfigureServices(
                            services => { services.AddMvc(options => options.EnableEndpointRouting = false); })
                        .Configure(
                            app =>
                            {
                                app.UseMiddleware<UnexpectedAggregateVersionMiddleware>();
                                app.UseMvc(builder => builder.MapGet(Route, _ => throw new UnexpectedAggregateVersionException()));
                            });
                })
            .StartAsync();
    }

    [Fact]
    public async Task Then_It_Returns_A_412_Response()
    {
        var testClient = _host.GetTestClient();

        var response = await testClient.GetAsync(Route);

        response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
    }

    public Task DisposeAsync()
    {
        _host.Dispose();
        return Task.CompletedTask;
    }
}
