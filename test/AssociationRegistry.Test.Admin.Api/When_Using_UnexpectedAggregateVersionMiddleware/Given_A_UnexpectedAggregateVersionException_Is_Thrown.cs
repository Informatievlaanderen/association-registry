﻿namespace AssociationRegistry.Test.Admin.Api.When_Using_UnexpectedAggregateVersionMiddleware;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using EventStore;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net;
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
                                                               services =>
                                                               {
                                                                   services.AddMvc(options => options.EnableEndpointRouting = false);
                                                                   services.AddSingleton(new StartupConfigureOptions());
                                                                   services.AddScoped<ProblemDetailsHelper>();
                                                               })
                                                          .Configure(
                                                               app =>
                                                               {
                                                                   app.UseMiddleware<UnexpectedAggregateVersionMiddleware>();

                                                                   app.UseMvc(builder => builder.MapGet(
                                                                                  Route,
                                                                                  handler: _
                                                                                      => throw new UnexpectedAggregateVersionException()));
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

        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetails.Detail.Should().Be(ValidationMessages.Status412PreconditionFailed);
    }

    public Task DisposeAsync()
    {
        _host.Dispose();

        return Task.CompletedTask;
    }
}
