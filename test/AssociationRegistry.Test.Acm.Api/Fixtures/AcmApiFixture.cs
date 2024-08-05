namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using AssociationRegistry.Acm.Api;
using AssociationRegistry.Acm.Api.Constants;
using AssociationRegistry.Acm.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Acm.Api.Infrastructure.Extensions;
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
using NodaTime;
using Npgsql;
using Oakton;
using Polly;
using System.Net.Http.Headers;
using System.Reflection;
using Xunit;

public abstract class AcmApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "acmapifixture";
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public IDocumentStore DocumentStore
        => _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();

    public EventConflictResolver EventConflictResolver
        => new EventConflictResolver(Array.Empty<IEventPreConflictResolutionStrategy>() ,Array.Empty<IEventPostConflictResolutionStrategy>());

    public AcmApiClient AcmApiClient
        => Clients.Authenticated;

    public AcmApiClient UnauthenticatedClient
        => Clients.Unauthenticated;

    public IServiceProvider ServiceProvider
        => _webApplicationFactory.Services;

    protected AcmApiFixture()
    {
        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AcmApiFixture>(),
                    GetConnectionString(GetConfiguration(), RootDatabase))
               .GetAwaiter().GetResult();

        EnsureDbExists(GetConfiguration());

        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AcmApiFixture>(),
                    GetConnectionString(GetConfiguration(), GetConfiguration().GetPostgreSqlOptionsSection().Database!))
               .GetAwaiter().GetResult();

        OaktonEnvironment.AutoStartHost = true;

        _webApplicationFactory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);

                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                               .AddJsonFile(path: "appsettings.json", optional: true)
                               .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                               .AddInMemoryCollection(
                                    new[]
                                    {
                                        new KeyValuePair<string, string>(
                                            $"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier),
                                    })
                    );
                });

        var postgreSqlOptionsSection = _webApplicationFactory.Services.GetRequiredService<PostgreSqlOptionsSection>();

        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AcmApiFixture>(), GetRootConnectionString(postgreSqlOptionsSection))
               .GetAwaiter().GetResult();

        Clients = new Clients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                              .Get<OAuth2IntrospectionOptions>(),
            _webApplicationFactory.CreateClient);
    }

    public Clients Clients { get; }

    public AcmApiClient DefaultClient
        => Clients.Authenticated;

    public async Task InitializeAsync()
        => await Given();

    public virtual Task DisposeAsync()
        => Task.CompletedTask;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Clients.SafeDispose();
        _webApplicationFactory.SafeDispose();
        DropDatabase();
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

    private static string GetRootDirectoryOrThrow()
    {
        var maybeRootDirectory = Directory
                                .GetParent(Assembly.GetExecutingAssembly().Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
    }

    protected async Task<StreamActionResult> AddEvents(string vCode, IEvent[] eventsToAdd, CommandMetadata? metadata = null)
    {
        if (!eventsToAdd.Any())
            return StreamActionResult.Empty;

        if (DocumentStore is null)
            throw new NullReferenceException("DocumentStore cannot be null when adding an event");

        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();

        if (daemon is null)
            throw new NullReferenceException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant(), Guid.NewGuid());

        var eventStore = new EventStore(DocumentStore, EventConflictResolver, NullLogger<EventStore>.Instance);
        var result = StreamActionResult.Empty;

        foreach (var @event in eventsToAdd)
        {
            result = await eventStore.Save(vCode.ToUpperInvariant(), metadata, CancellationToken.None, @event);
        }

        var retry = Policy
                   .Handle<Exception>()
                   .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromSeconds(10 * i));

        await retry.ExecuteAsync(
            async () => { await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(value: 60)); });

        return result;
    }

    private IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
                     .SetBasePath(GetRootDirectory())
                     .AddJsonFile(path: "appsettings.json", optional: true)
                     .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] = _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;

        return tempConfiguration;
    }

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
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

        Authenticated = new AcmApiClient(CreateMachine2MachineClientFor(clientId: "acmClient", Security.Scopes.ACM, clientSecret: "secret")
                                        .GetAwaiter().GetResult());

        Unauthenticated = new AcmApiClient(_createClientFunc());

        Unauthorized = new AcmApiClient(CreateMachine2MachineClientFor(clientId: "acmClient", Security.Scopes.Info, clientSecret: "secret")
                                       .GetAwaiter().GetResult());
    }

    public AcmApiClient Authenticated { get; }
    public AcmApiClient Unauthenticated { get; }
    public AcmApiClient Unauthorized { get; }

    public void Dispose()
    {
        Authenticated.SafeDispose();
        Unauthenticated.SafeDispose();
        Unauthorized.SafeDispose();
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
