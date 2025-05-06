namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

using Amazon.SQS;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Constants;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.NutsLau;
using Common.Framework;
using Events;
using Helpers;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using JasperFx.Core;
using Marten;
using Marten.Events.Daemon.Coordination;
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
using System.Text;
using Xunit;
using ProjectionHostProgram = AssociationRegistry.Admin.ProjectionHost.Program;

public abstract class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier;
    private readonly WebApplicationFactory<Program> _adminApiServer;
    private readonly WebApplicationFactory<ProjectionHostProgram> _projectionHostServer;
    public IConfigurationRoot Configuration { get; }

    internal IElasticClient ElasticClient
        => (IElasticClient)_adminApiServer.Services.GetRequiredService(typeof(ElasticClient));

    public IDocumentStore DocumentStore
        => _adminApiServer.Services.GetRequiredService<IDocumentStore>();

    public EventConflictResolver EventConflictResolver
        => _adminApiServer.Services.GetRequiredService<EventConflictResolver>();

    public AdminApiClient AdminApiClient
        => new(AdminApiClients.GetAuthenticatedHttpClient());

    public AdminApiClient SuperAdminApiClient
        => AdminApiClients.SuperAdmin;

    private string VerenigingenIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:Verenigingen"];

    private string DuplicateDetectionIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:DuplicateDetection"];

    protected AdminApiFixture(string?  identifier = "adminapifixture")
    {
        _identifier = identifier;
        Configuration = GetConfiguration();

        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AdminApiFixture>(),
                    GetConnectionString(Configuration, RootDatabase))
               .GetAwaiter().GetResult();

        DropDatabase();
        EnsureDbExists(Configuration);

        OaktonEnvironment.AutoStartHost = true;

        _adminApiServer = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting(key: "PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(Configuration);
                    builder.UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        InsertWerkingsGebieden().GetAwaiter().GetResult();

        _adminApiServer.CreateClient();

        SqsClientWrapper = _adminApiServer.Services.GetRequiredService<ISqsClientWrapper>();
        AmazonSqs = _adminApiServer.Services.GetRequiredService<IAmazonSQS>();

        AdminApiClients = new AdminApiClients(
            Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                             .Get<OAuth2IntrospectionOptions>(),
            _adminApiServer.CreateClient);

        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AdminApiFixture>(),
                    GetConnectionString(Configuration, Configuration.GetPostgreSqlOptionsSection().Database!))
               .GetAwaiter().GetResult();

        var postgreSqlOptionsSection = _adminApiServer.Services.GetRequiredService<PostgreSqlOptionsSection>();

        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AdminApiFixture>(), GetRootConnectionString(postgreSqlOptionsSection))
               .GetAwaiter().GetResult();

        WaitFor.ElasticSearchToBecomeAvailable(ElasticClient, new NullLogger<AdminApiFixture>())
               .GetAwaiter().GetResult();

        if (ElasticClient.Indices.Exists(DuplicateDetectionIndexName).Exists)
        {
            ElasticClient.Indices.Delete(DuplicateDetectionIndexName);
        }

        _projectionHostServer = new WebApplicationFactory<ProjectionHostProgram>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);
                    builder.UseConfiguration(Configuration);
                    builder.UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", _identifier);
                });
    }

    private async Task InsertWerkingsGebieden()
    {
        await using var session = DocumentStore.LightweightSession();

        var werkingsgebieden = WerkingsgebiedenServiceMock.All
                                                          .Where(w => w.Code.Length > 8) // only detailed werkingsgebieden
                                                          .Select((w, index) =>
                                                           {
                                                               var nuts = w.Code.Substring(0, 5);
                                                               var lau = w.Code.Substring(5);
                                                               var postcode = (1000 + index).ToString();

                                                               return new PostalNutsLauInfo
                                                               {
                                                                   Postcode = postcode,
                                                                   Gemeentenaam = w.Naam,
                                                                   Nuts = nuts,
                                                                   Lau = lau
                                                               };
                                                           });

        session.StoreObjects(werkingsgebieden);
        await session.SaveChangesAsync();
    }

    public IAmazonSQS AmazonSqs { get; set; }
    public ISqsClientWrapper SqsClientWrapper { get; set; }

    public IDocumentStore ApiDocumentStore
        => _adminApiServer.Services.GetRequiredService<IDocumentStore>();

    public IDocumentStore ProjectionsDocumentStore
        => _projectionHostServer.Services.GetRequiredService<IDocumentStore>();

    public IProjectionCoordinator ProjectionCoordinator
        => _projectionHostServer.Services.GetRequiredService<IProjectionCoordinator>();

    public AdminApiClient UnauthenticatedClient
        => AdminApiClients.Unauthenticated;

    public IServiceProvider ServiceProvider
        => _adminApiServer.Services;

    public AdminApiClients AdminApiClients { get; }

    public AdminApiClient DefaultClient
        => AdminApiClients.Authenticated;

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
        AdminApiClients.SafeDispose();

        _adminApiServer.SafeDispose();
        _projectionHostServer.SafeDispose();
    }

    private static void EnsureDbExists(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection = configuration.GetPostgreSqlOptionsSection();
        using var connection = new NpgsqlConnection(GetConnectionString(configuration, RootDatabase));

        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            cmd.CommandText += $"CREATE DATABASE {postgreSqlOptionsSection.Database} WITH OWNER = {postgreSqlOptionsSection.Username};";
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
        var result = await eventStore.Save(vCode.ToUpperInvariant(), 0, metadata, CancellationToken.None, eventsToAdd);

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

public class AdminApiClients : IDisposable
{
    private readonly Func<HttpClient> _createClientFunc;
    private readonly OAuth2IntrospectionOptions _oAuth2IntrospectionOptions;

    public AdminApiClients(OAuth2IntrospectionOptions oAuth2IntrospectionOptions, Func<HttpClient> createClientFunc)
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
