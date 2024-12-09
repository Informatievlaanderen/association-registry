﻿namespace AssociationRegistry.Test.Projections.Framework;

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

public class ProjectionContext : IProjectionContext, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private IConfigurationRoot Configuration { get; }
    public IDocumentStore AdminStore { get; set; }
    public IDocumentStore PublicStore { get; set; }
    public IElasticClient ElasticClient { get; set; }

    public ProjectionContext()
    {
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.v2.beheer.json").Build();
    }

    public async Task InitializeAsync()
    {
        DropDatabase(Configuration);
        EnsureDbExists(Configuration);

        var adminStore = DocumentStore.For(
            opts =>
            {
                ConfigureMartenExtensions.ConfigureStoreOptions(opts,
                                                                NullLogger<LocatieLookupProjection>.Instance,
                                                                NullLogger<LocatieZonderAdresMatchProjection>.Instance,
                                                                null,
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

        var publicStore = DocumentStore.For(
            opts =>
            {
                Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder.ConfigureMartenExtensions.ConfigureStoreOptions(
                    opts,
                    null,
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

    public Task DisposeAsync() => Task.CompletedTask;
}
