namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Admin.Api;
using Alba;
using AlbaHost;
using AssociationRegistry.Framework;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using Oakton;
using TestClasses;
using Vereniging;
using Xunit;
using ProjectionHostProgram = Public.ProjectionHost.Program;

public class FullBlownApiSetup: IAsyncLifetime, IApiSetup
{
    private string _schema;

    public FullBlownApiSetup()
    {

    }

    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }
    public IAlbaHost PublicProjectionHost { get; private set; }
    public IAlbaHost PublicApiHost { get; private set; }

    public async Task InitializeAsync()
    {
        _schema = "fullblowne2e";
        OaktonEnvironment.AutoStartHost = true;

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json").Build();

        AdminApiHost = (await AlbaHost.For<Program>(ConfigureForTesting(configuration, _schema)))
           .EnsureEachCallIsAuthenticated();
        AdminProjectionHost = await AlbaHost.For<Admin.ProjectionHost.Program>(ConfigureForTesting(configuration, _schema));

        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        PublicProjectionHost = await AlbaHost.For<ProjectionHostProgram>(ConfigureForTesting(configuration, _schema));
        PublicApiHost = await AlbaHost.For<Public.Api.Program>(ConfigureForTesting(configuration, _schema));

        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await AdminProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await PublicProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await PublicApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await PublicProjectionHost.ResumeAllDaemonsAsync();
        await AdminProjectionHost.ResumeAllDaemonsAsync();
    }

    private Action<IWebHostBuilder> ConfigureForTesting(IConfigurationRoot configuration, string schema)
    {
        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
                  services.Configure<PostgreSqlOptionsSection>(s => { s.Schema = schema; });
              })
             .UseSetting(key: "ASPNETCORE_ENVIRONMENT", value: "Development")
             .UseSetting(key: "ApplyAllDatabaseChangesDisabled", value: "true")
             .UseSetting($"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Schema)}", schema)
             .UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", $"public_{schema.ToLowerInvariant()}");
        };
    }

    public async Task DisposeAsync()
    {
        await AdminApiHost.StopAsync();
        await PublicApiHost.StopAsync();
        await AdminProjectionHost.StopAsync();
        await PublicProjectionHost.StopAsync();
        await AdminApiHost.DisposeAsync();
        await PublicApiHost.DisposeAsync();
        await PublicProjectionHost.DisposeAsync();
        await AdminProjectionHost.DisposeAsync();
    }

    public async Task ExecuteGiven(IScenario emptyScenario)
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();
        session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
        session.CorrelationId = Guid.NewGuid().ToString();

        foreach (var eventsPerStream in await emptyScenario.GivenEvents(AdminApiHost.Services.GetRequiredService<IVCodeService>()))
        {
            session.Events.Append(eventsPerStream.Key, eventsPerStream.Value);
        }

        await session.SaveChangesAsync();

        await AdminProjectionHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await PublicProjectionHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
