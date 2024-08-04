namespace AssociationRegistry.Test.E2E;

using Admin.Api;
using Admin.Api.Infrastructure.Extensions;
using Alba;
using Framework.AlbaHost;
using Hosts;
using Hosts.Configuration.ConfigurationBindings;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oakton;
using Xunit;
using Clients = Common.Clients.Clients;
using ProjectionHostProgram = Admin.ProjectionHost.Program;

public class AppFixture : IAsyncLifetime
{
    private readonly string _schema;
    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }

    public AppFixture(string schema)
    {
        _schema = schema;
    }

    public async Task InitializeAsync()
    {
        OaktonEnvironment.AutoStartHost = true;

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json").Build();

        AdminApiHost = (await AlbaHost.For<Program>(ConfigureForTesting(configuration)))
           .EnsureEachCallIsAuthenticated();

        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        AdminProjectionHost = await AlbaHost.For<ProjectionHostProgram>(ConfigureForTesting(configuration));

        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await AdminProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await AdminProjectionHost.ResumeAllDaemonsAsync();
    }

    private Action<IWebHostBuilder> ConfigureForTesting(IConfigurationRoot configuration)
    {
        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
                  services.Configure<PostgreSqlOptionsSection>(s => { s.Schema = _schema; });
              })
             .UseSetting(key: "ASPNETCORE_ENVIRONMENT", value: "Development");
        };
    }

    public async Task DisposeAsync()
    {
        await AdminApiHost.Services.GetRequiredService<IDocumentStore>().Advanced.ResetAllData();
        await AdminApiHost.DisposeAsync();
    }
}
