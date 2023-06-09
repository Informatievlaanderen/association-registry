namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using System.Net.Http.Headers;
using System.Reflection;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using EventStore;
using AssociationRegistry.Framework;
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
using Xunit;
using IEvent = global::AssociationRegistry.Framework.IEvent;
using ProjectionHostProgram = AssociationRegistry.Admin.ProjectionHost.Program;


public abstract class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "adminApiFixture";

    private readonly WebApplicationFactory<Program> _adminApiServer;
    private readonly WebApplicationFactory<ProjectionHostProgram> _projectionHostServer;

    public IDocumentStore ApiDocumentStore
        => _adminApiServer.Services.GetRequiredService<IDocumentStore>();

    public IDocumentStore ProjectionsDocumentStore
        => _projectionHostServer.Services.GetRequiredService<IDocumentStore>();

    public AdminApiClient AdminApiClient
        => Clients.Authenticated;

    public AdminApiClient UnauthenticatedClient
        => Clients.Unauthenticated;

    public IServiceProvider ServiceProvider
        => _adminApiServer.Services;

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
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                                .AddInMemoryCollection(
                                    new[]
                                    {
                                        new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier),
                                    })
                    );
                });

        var postgreSqlOptionsSection = _adminApiServer.Services.GetRequiredService<PostgreSqlOptionsSection>();
        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AdminApiFixture>(), GetRootConnectionString(postgreSqlOptionsSection))
            .GetAwaiter().GetResult();

        _projectionHostServer = new WebApplicationFactory<ProjectionHostProgram>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                                .AddInMemoryCollection(
                                    new[]
                                    {
                                        new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier),
                                    })
                    );
                });

        Clients = new Clients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>(),
            _adminApiServer.CreateClient);
    }

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

        if (ProjectionsDocumentStore is not { })
            throw new NullReferenceException("DocumentStore cannot be null when adding an event");

        using var daemon = await ProjectionsDocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();

        if (daemon is not { })
            throw new NullReferenceException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant());

        var eventStore = new EventStore(ProjectionsDocumentStore);
        var result = StreamActionResult.Empty;
        foreach (var @event in eventsToAdd) result = await eventStore.Save(vCode.ToUpperInvariant(), metadata, CancellationToken.None, @event);

        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retryCount: 3, i => TimeSpan.FromSeconds(10 * i));

        await retry.ExecuteAsync(
            async () => { await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(value: 60)); });

        return result;
    }

    private IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(GetRootDirectory())
            .AddJsonFile("appsettings.json", optional: true)
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

        Authenticated = new AdminApiClient(CreateMachine2MachineClientFor("vloketClient", AssociationRegistry.Admin.Api.Constants.Security.Scopes.Admin, "secret").GetAwaiter().GetResult());

        Unauthenticated = new AdminApiClient(_createClientFunc());

        Unauthorized = new AdminApiClient(CreateMachine2MachineClientFor("vloketClient", "vo_info", "secret").GetAwaiter().GetResult());
    }

    public AdminApiClient Authenticated { get; }

    public AdminApiClient Unauthenticated { get; }

    public AdminApiClient Unauthorized { get; }

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
