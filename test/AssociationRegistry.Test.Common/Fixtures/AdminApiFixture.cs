namespace AssociationRegistry.Test.Common.Fixtures;

using Admin.Api;
using Admin.Api.Constants;
using Admin.Api.Infrastructure.Extensions;
using Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using Common.Clients;
using Configuration;
using EventStore;
using Hosts.Configuration.ConfigurationBindings;
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
using System.Net.Http.Headers;
using Xunit;
using ProjectionHostProgram = Admin.ProjectionHost.Program;
using WaitFor = Hosts.WaitFor;

public abstract class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "adminapifixture";
    private readonly WebApplicationFactory<Program> _adminApiServer;
    private readonly WebApplicationFactory<ProjectionHostProgram> _projectionHostServer;

    internal IElasticClient ElasticClient
        => (IElasticClient)_adminApiServer.Services.GetRequiredService(typeof(ElasticClient));

    public IDocumentStore DocumentStore
        => _adminApiServer.Services.GetRequiredService<IDocumentStore>();

    public EventConflictResolver EventConflictResolver
        => _adminApiServer.Services.GetRequiredService<EventConflictResolver>();

    public AdminApiClient AdminApiClient
        => new(Clients.GetAuthenticatedHttpClient());

    public AdminApiClient SuperAdminApiClient
        => Clients.SuperAdmin;

    private string VerenigingenIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:Verenigingen"];

    private string DuplicateDetectionIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:DuplicateDetection"];

    protected AdminApiFixture()
    {
        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AdminApiFixture>(),
                    GetConnectionString(GetConfiguration(), RootDatabase))
               .GetAwaiter().GetResult();

        DropDatabase();
        EnsureDbExists(GetConfiguration());

        OaktonEnvironment.AutoStartHost = true;

        _adminApiServer = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting(key: "PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        _adminApiServer.CreateClient();

        Clients = new Clients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                              .Get<OAuth2IntrospectionOptions>(),
            _adminApiServer.CreateClient);

        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AdminApiFixture>(),
                    GetConnectionString(GetConfiguration(), GetConfiguration().GetPostgreSqlOptionsSection().Database!))
               .GetAwaiter().GetResult();

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
                    builder.UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        ConfigureElasticClient(ElasticClient, VerenigingenIndexName, DuplicateDetectionIndexName);
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
    {
        await Given();
    }

    public virtual async Task DisposeAsync()
    {
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Clients.SafeDispose();

        _adminApiServer.SafeDispose();
        _projectionHostServer.SafeDispose();
    }

    public static void EnsureDbExists(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection = configuration.GetPostgreSqlOptionsSection();
        using var connection = new NpgsqlConnection(GetConnectionString(configuration, RootDatabase));

        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();

            cmd.CommandText += $@"
DO $$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_database
      WHERE datname = '{postgreSqlOptionsSection.Database}'
   ) THEN
      CREATE DATABASE {postgreSqlOptionsSection.Database} WITH OWNER = {postgreSqlOptionsSection.Username};
   END IF;
END
$$;
";
            cmd.ExecuteNonQuery();
        }
        catch (PostgresException ex)
        {
            if (ex.MessageText != $"database \"{postgreSqlOptionsSection.Database.ToLower()}\" already exists")
                throw;
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private static void ConfigureElasticClient(
        IElasticClient client,
        string verenigingenIndexName,
        string duplicateDetectionIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.CreateVerenigingIndex(verenigingenIndexName);

        if (client.Indices.Exists(duplicateDetectionIndexName).Exists)
            client.Indices.Delete(duplicateDetectionIndexName);

        client.Indices.CreateDuplicateDetectionIndex(duplicateDetectionIndexName);

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

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant(), Guid.NewGuid());

        var eventStore = new EventStore(ProjectionsDocumentStore, EventConflictResolver, NullLogger<EventStore>.Instance);
        var result = await eventStore.Save(vCode.ToUpperInvariant(), metadata, CancellationToken.None, eventsToAdd);

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
        => CreateMachine2MachineClientFor(clientId: "vloketClient", Security.Scopes.Admin, clientSecret: "secret").GetAwaiter().GetResult();

    private HttpClient GetSuperAdminHttpClient()
        => CreateMachine2MachineClientFor(clientId: "superAdminClient", Security.Scopes.Admin, clientSecret: "secret").GetAwaiter()
           .GetResult();

    public AdminApiClient Authenticated
        => new(GetAuthenticatedHttpClient());

    public AdminApiClient SuperAdmin
        => new(GetSuperAdminHttpClient());

    public AdminApiClient Unauthenticated
        => new(_createClientFunc());

    public AdminApiClient Unauthorized
        => new(CreateMachine2MachineClientFor(clientId: "vloketClient", scope: "vo_info", clientSecret: "secret").GetAwaiter().GetResult());

    public void Dispose()
    {
    }

    private async Task<HttpClient> CreateMachine2MachineClientFor(
        string clientId,
        string scope,
        string clientSecret)
    {
        var tokenClient = new TokenClient(
            client: () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{_oAuth2IntrospectionOptions.Authority}/connect/token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>(key: "scope", scope),
                    }),
            });

        var acmResponse = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        var token = acmResponse.AccessToken;
        var httpClientFor = _createClientFunc();
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", token);

        return httpClientFor;
    }
}
