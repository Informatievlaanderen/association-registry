namespace AssociationRegistry.Test.E2E;

using Alba;
using Admin.Api;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Common.Fixtures;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oakton;
using Polly;
using Polly.Retry;
using Xunit;
using Clients = Common.Clients.Clients;
using ProjectionHostProgram = Admin.ProjectionHost.Program;

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

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json").Build();

        AdminApiFixture.EnsureDbExists(configuration);

        Host = await AlbaHost.For<Program>(b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
                  // services.Configure<PostgreSqlOptionsSection>(s => { s.Schema = SchemaName; });
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
           .WithWebHostBuilder(b =>
            {
                b.UseEnvironment("Development");
                b.UseContentRoot(Directory.GetCurrentDirectory());

                b.UseConfiguration(configuration);

                b.ConfigureServices((context, services) =>
                  {
                      context.HostingEnvironment.EnvironmentName = "Development";
                      // services.Configure<PostgreSqlOptionsSection>(s => { s.Schema = SchemaName; });
                  })
                 .UseSetting("ASPNETCORE_ENVIRONMENT", "Development");
            });

        var store = Host.Services.GetRequiredService<IDocumentStore>();
        await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await ProjectionHostServer.Services.GetRequiredService<IDocumentStore>().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

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
