namespace AssociationRegistry.Test.Projections.Framework;

using Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;
using Admin.ProjectionHost.Projections.Locaties;
using Admin.ProjectionHost.Projections.Search;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using Npgsql;
using Public.ProjectionHost.Projections.Search;
using ElasticRepository = Admin.ProjectionHost.Projections.Search.ElasticRepository;

public class ProjectionContext : IProjectionContext, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private IConfigurationRoot Configuration { get; }
    public IDocumentStore AdminStore { get; set; }
    public IDocumentStore PublicStore { get; set; }
    public IDocumentStore AcmStore { get; set; }
    public IElasticClient AdminElasticClient { get; set; }
    public IElasticClient AdminProjectionElasticClient { get; set; }
    public IElasticClient PublicElasticClient { get; set; }
    public IElasticClient PublicProjectionElasticClient { get; set; }

    public ProjectionContext()
    {
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.v2.beheer.json").Build();
    }

    public async ValueTask InitializeAsync()
    {
        // DropDatabase(Configuration);
        EnsureDbExists(Configuration);

        AdminElasticClient =
            Admin.Api.Infrastructure.Extensions.ElasticSearchExtensions.CreateElasticClient(
                Configuration.GetElasticSearchOptionsSection(), NullLogger.Instance);

        AdminProjectionElasticClient =
            Admin.ProjectionHost.Infrastructure.Extensions.ElasticSearchExtensions.CreateElasticClient(
                Configuration.GetElasticSearchOptionsSection(), NullLogger.Instance);

        var adminStore = DocumentStore.For(
            opts =>
            {
                ConfigureMartenExtensions.ConfigureStoreOptions(opts,
                                                                NullLogger<LocatieLookupProjection>.Instance,
                                                                NullLogger<LocatieZonderAdresMatchProjection>.Instance,
                                                                new ElasticRepository(AdminProjectionElasticClient),
                                                                true,
                                                                NullLogger<BeheerZoekenEventsConsumer>.Instance,
                                                                new PostgreSqlOptionsSection()
                                                                {
                                                                    Host = "localhost",
                                                                    Database = RootDatabase,
                                                                    Password = "root",
                                                                    Username = "root",
                                                                    Schema = "admin",
                                                                });
            });

        await adminStore.Advanced.Clean.DeleteAllEventDataAsync();

        AdminStore = adminStore;

        PublicElasticClient =
            Public.Api.Infrastructure.Extensions.ElasticSearchExtensions.CreateElasticClient(
                Configuration.GetElasticSearchOptionsSection(), NullLogger.Instance);

        PublicProjectionElasticClient =
            Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder.ConfigureElasticSearchExtensions.CreateElasticClient(
                Configuration.GetElasticSearchOptionsSection());

        var publicStore = DocumentStore.For(
            opts =>
            {
                Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder.ConfigureMartenExtensions.ConfigureStoreOptions(
                    opts,
                    new AssociationRegistry.Public.ProjectionHost.Projections.Search.ElasticRepository(PublicProjectionElasticClient),
                    NullLogger<MartenEventsConsumer>.Instance,
                    new PostgreSqlOptionsSection()
                    {
                        Host = "localhost",
                        Database = RootDatabase,
                        Password = "root",
                        Username = "root",
                        Schema = "admin",
                    },
                    true);
            });

        await publicStore.Advanced.Clean.DeleteAllEventDataAsync();

        PublicStore = publicStore;


        var acmStore = DocumentStore.For(
            opts =>
            {
                AssociationRegistry.Acm.Api.Infrastructure.Extensions.MartenExtensions.ConfigureStoreOptions(
                    opts,
                    new PostgreSqlOptionsSection()
                    {
                        Host = "localhost",
                        Database = RootDatabase,
                        Password = "root",
                        Username = "root",
                        Schema = "admin",
                    },
                    true);
            });

        await acmStore.Advanced.Clean.DeleteAllEventDataAsync();

        AcmStore = acmStore;
    }

    public async Task SaveAsync(EventsPerVCode[] events, IDocumentSession session)
    {
        foreach (var eventsPerVCode in events)
        {
            session.Events.Append(eventsPerVCode.VCode, eventsPerVCode.Events);
            await session.SaveChangesAsync();
        }
    }

    private void DropDatabase(IConfigurationRoot configuration)
    {
        using var connection = new NpgsqlConnection(GetConnectionString(configuration, RootDatabase));
        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText += $"DROP DATABASE IF EXISTS {configuration["PostgreSQLOptions:database"]} WITH (FORCE);";
            cmd.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
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

    private static string GetConnectionString(IConfiguration configurationRoot, string database)
        => $"host={configurationRoot["PostgreSQLOptions:host"]};" +
           $"database={database};" +
           $"password={configurationRoot["PostgreSQLOptions:password"]};" +
           $"username={configurationRoot["PostgreSQLOptions:username"]}";

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
