namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using System.Net.Http.Headers;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using EventStore;
using Framework.Helpers;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using NodaTime;
using Npgsql;
using Oakton;
using Polly;
using Xunit;
using Policy = Polly.Policy;
using ProjectionHostProgram = AssociationRegistry.Admin.ProjectionHost.Program;

public abstract class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "adminapifixture";

    private readonly WebApplicationFactory<Program> _adminApiServer;

    private readonly WebApplicationFactory<ProjectionHostProgram> _projectionHostServer;

    private IElasticClient ElasticClient
        => (IElasticClient)_adminApiServer.Services.GetRequiredService(typeof(ElasticClient));

    public IDocumentStore DocumentStore
        => _adminApiServer.Services.GetRequiredService<IDocumentStore>();

    public AdminApiClient AdminApiClient
        => new(Clients.GetAuthenticatedHttpClient());

    private string VerenigingenIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:Verenigingen"];

    protected AdminApiFixture()
    {
        WaitFor.PostGreSQLToBecomeAvailable(
                new NullLogger<AdminApiFixture>(),
                GetConnectionString(GetConfiguration(), RootDatabase))
            .GetAwaiter().GetResult();

        DropDatabase();

        EnsureDbExists(GetConfiguration().GetPostgreSqlOptionsSection());

        WaitFor.PostGreSQLToBecomeAvailable(
                new NullLogger<AdminApiFixture>(),
                GetConnectionString(GetConfiguration(), GetConfiguration().GetPostgreSqlOptionsSection().Database!))
            .GetAwaiter().GetResult();

        OaktonEnvironment.AutoStartHost = true;

        _adminApiServer = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting("PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.UseSetting("ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        var postgreSqlOptionsSection = _adminApiServer.Services.GetRequiredService<PostgreSqlOptionsSection>();
        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AdminApiFixture>(), GetRootConnectionString(postgreSqlOptionsSection))
            .GetAwaiter().GetResult();

        WaitFor.ElasticSearchToBecomeAvailable(ElasticClient, new NullLogger<AdminApiFixture>())
            .GetAwaiter().GetResult();

        _projectionHostServer = new WebApplicationFactory<ProjectionHostProgram>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.UseSetting("ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        ConfigureElasticClient(ElasticClient, VerenigingenIndexName);

        Clients = new Clients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>(),
            _adminApiServer.CreateClient);
    }

    public IDocumentStore ApiDocumentStore
        => _adminApiServer.Services.GetRequiredService<IDocumentStore>();

    public IDocumentStore ProjectionsDocumentStore
        => _projectionHostServer.Services.GetRequiredService<IDocumentStore>();

    public AdminApiClient UnauthenticatedClient
        => Clients.Unauthenticated;

    public IServiceProvider ServiceProvider
        => _adminApiServer.Services;

    public Clients Clients { get; }

    public AdminApiClient DefaultClient
        => Clients.Authenticated;

    public async Task InitializeAsync()
        => await Given();

    public virtual Task DisposeAsync()
        => Task.CompletedTask;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Clients.SafeDispose();
        _adminApiServer.SafeDispose();
        _projectionHostServer.SafeDispose();
        DropDatabase();
    }

    private static void EnsureDbExists(PostgreSqlOptionsSection postgreSqlOptionsSection)
    {
        using var documentStore = Marten.DocumentStore.For(
            options =>
            {
                options.Connection(postgreSqlOptionsSection.GetConnectionString());
                options.CreateDatabasesForTenants(
                    databaseConfig =>
                    {
                        databaseConfig.MaintenanceDatabase(GetRootConnectionString(postgreSqlOptionsSection));
                        databaseConfig.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner(postgreSqlOptionsSection.Username!);
                    });
                options.RetryPolicy(DefaultRetryPolicy.Times(maxRetryCount: 5, _ => true, i => TimeSpan.FromSeconds(i)));
            });
    }

    private static void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.CreateVerenigingIndex(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
    }

    private static string GetRootConnectionString(PostgreSqlOptionsSection postgreSqlOptionsSection)
        => $"host={postgreSqlOptionsSection.Host}:5432;" +
           "database=postgres;" +
           $"password={postgreSqlOptionsSection.Password};" +
           $"username={postgreSqlOptionsSection.Username}";

    protected async Task<StreamActionResult> AddEvents(string vCode, IEvent[] eventsToAdd, CommandMetadata? metadata = null)
    {
        if (!eventsToAdd.Any())
            return StreamActionResult.Empty;

        if (ProjectionsDocumentStore is null)
            throw new NullReferenceException("DocumentStore cannot be null when adding an event");

        using var daemon = await ProjectionsDocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();

        if (daemon is null)
            throw new NullReferenceException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant(), Guid.NewGuid());

        var eventStore = new EventStore(ProjectionsDocumentStore);
        var result = await eventStore.Save(vCode.ToUpperInvariant(), metadata, CancellationToken.None, eventsToAdd);

        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retryCount: 3, i => TimeSpan.FromSeconds(10 * i));

        await retry.ExecuteAsync(
            async () => { await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(value: 60)); });

        await ElasticClient.Indices.RefreshAsync(Indices.All);

        return result;
    }

    private IConfigurationRoot GetConfiguration()
    {
        var tempConfiguration = ConfigurationHelper.GetConfiguration();
        tempConfiguration["PostgreSQLOptions:database"] = _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;
        return tempConfiguration;
    }

    private void DropDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString(GetConfiguration(), RootDatabase));
        using var cmd = connection.CreateCommand();
        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText += $"DROP DATABASE IF EXISTS \"{GetConfiguration()["PostgreSQLOptions:database"]}\" WITH (FORCE);";
            cmd.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private static string GetConnectionString(IConfiguration configurationRoot, string database)
        => $"host={configurationRoot["PostgreSQLOptions:host"]};" +
           $"database={database};" +
           $"password={configurationRoot["PostgreSQLOptions:password"]};" +
           $"username={configurationRoot["PostgreSQLOptions:username"]}";

    protected abstract Task Given();
}

public class Clients : IDisposable
{
    private readonly Func<HttpClient> _createClientFunc;
    private readonly OAuth2IntrospectionOptions _oAuth2IntrospectionOptions;

    public Clients(OAuth2IntrospectionOptions oAuth2IntrospectionOptions, Func<HttpClient> createClientFunc)
    {
        _oAuth2IntrospectionOptions = oAuth2IntrospectionOptions;
        _createClientFunc = createClientFunc;
    }

    public HttpClient GetAuthenticatedHttpClient()
        => CreateMachine2MachineClientFor("vloketClient", Security.Scopes.Admin, "secret").GetAwaiter().GetResult();

    public AdminApiClient Authenticated
        => new(GetAuthenticatedHttpClient());

    public AdminApiClient Unauthenticated
        => new(_createClientFunc());

    public AdminApiClient Unauthorized
        => new(CreateMachine2MachineClientFor("vloketClient", "vo_info", "secret").GetAwaiter().GetResult());

    public void Dispose()
    {
    }

    private async Task<HttpClient> CreateMachine2MachineClientFor(
        string clientId,
        string scope,
        string clientSecret)
    {
        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{_oAuth2IntrospectionOptions.Authority}/connect/token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var acmResponse = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        var token = acmResponse.AccessToken;
        var httpClientFor = _createClientFunc();
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return httpClientFor;
    }
}
