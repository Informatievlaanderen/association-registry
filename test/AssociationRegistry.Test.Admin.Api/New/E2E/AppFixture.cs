using Alba;
using AssociationRegistry.Test.Admin.Api.New;
using Marten;
using Xunit;

namespace AssociationRegistry.Test.Admin.Api.New;

using Alba;
using Alba.Security;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.ProjectionHost.Constants;
using Fixtures;
using IdentityModel.AspNetCore.OAuth2Introspection;
using JasperFx.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Oakton;
using Polly;
using Polly.Retry;
using Public.ProjectionHost.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;
using ProjectionHostProgram = AssociationRegistry.Admin.ProjectionHost.Program;

public class AppFixture : IAsyncLifetime
{
    public const string Initiator = "V0001001";
    public string? AuthCookie { get; private set; }

    public AsyncRetryPolicy RetryPolicy;
    public WebApplicationFactory<ProjectionHostProgram> ProjectionHostServer;
    private string SchemaName { get; } = "sch" + Guid.NewGuid().ToString().Replace("-", string.Empty);

    public IAlbaHost Host { get; private set; }

    public async Task InitializeAsync()
    {
        OaktonEnvironment.AutoStartHost = true;

        Host = await AlbaHost.For<Program>(b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
                  services.Configure<PostgreSqlOptionsSection>(s => { s.Schema = SchemaName; });
              })
             .UseSetting("ASPNETCORE_ENVIRONMENT", "Development");
        });

        var adminApiClient = new Clients(Host.Services.GetRequiredService<OAuth2IntrospectionOptions>(), () => new HttpClient())
           .Authenticated;

        Host.BeforeEach(context =>
        {
            context.Request.Headers["x-correlation-id"] = Guid.NewGuid().ToString();
            context.Request.Headers["vr-initiator"] = Initiator;

            context.Request.Headers["Authorization"] =
                adminApiClient.HttpClient.DefaultRequestHeaders.GetValues("Authorization").First();
        });

        ProjectionHostServer = new WebApplicationFactory<ProjectionHostProgram>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.ConfigureServices((context, services) =>
                            {
                                context.HostingEnvironment.EnvironmentName = "Development";
                                services.Configure<AssociationRegistry.Admin.ProjectionHost.Infrastructure.ConfigurationBindings.PostgreSqlOptionsSection>(s => { s.Schema = SchemaName; });
                            })
                           .UseSetting("ASPNETCORE_ENVIRONMENT", "Development");
                });


        var store = Host.Services.GetRequiredService<IDocumentStore>();
        await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        RetryPolicy = Policy
                     .Handle<Exception>()
                     .WaitAndRetryAsync(3, retryAttempt =>
                                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential backoff
                                      , (exception, timeSpan, context) =>
                                        {
                                            Console.WriteLine(
                                                $"An error occurred: {exception.Message}. Waiting {timeSpan} before next retry.");
                                        });
    }

    public async Task DisposeAsync()
    {
        await Host.Services.GetRequiredService<IDocumentStore>().Advanced.ResetAllData();
        await Host.DisposeAsync();
    }
}
