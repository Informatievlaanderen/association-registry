﻿namespace AssociationRegistry.Test.Projections.Framework;

using Alba;
using AssociationRegistry.Framework;
using Hosts.Configuration;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using Oakton;
using AdminProjectionHostProgram = Admin.ProjectionHost.Program;
using PublicProjectionHostProgram = Public.ProjectionHost.Program;

public class ProjectionContext : IProjectionContext, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private IConfigurationRoot Configuration { get; }

    public ProjectionContext()
    {
        OaktonEnvironment.AutoStartHost = true;
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.v2.beheer.json").Build();

        MetadataInitiator = "metadata.Initiator";
        MetadataTijdstip = InstantPattern.General.Format(new Instant());
    }

    public ProjectionHostContext Beheer { get; private set; }
    public ProjectionHostContext Publiek { get; private set; }
    public string MetadataInitiator { get; }

    public string MetadataTijdstip { get; }

    public async Task InitializeAsync()
    {
        // DropDatabase(Configuration);
        EnsureDbExists(Configuration);

        var beheerProjectionHost = await AlbaHost.For<AdminProjectionHostProgram>(ConfigureForTesting(Configuration));
        var publiekProjectionHost = await AlbaHost.For<PublicProjectionHostProgram>(ConfigureForTesting(Configuration));

        Beheer = new(beheerProjectionHost);
        Publiek = new(publiekProjectionHost);

        await InitializeHostAsync(beheerProjectionHost);
        await InitializeHostAsync(publiekProjectionHost);
    }

    private async Task InitializeHostAsync(IAlbaHost host)
    {
        await host.DocumentStore().Advanced.ResetAllData();
        await host.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await host.ResumeAllDaemonsAsync();
    }

    public async Task SaveAsync(EventsPerVCode[] events)
    {
        await using var session = Beheer.Host.DocumentStore().LightweightSession();
        foreach (var eventsPerVCode in events) session.Events.Append(eventsPerVCode.VCode, eventsPerVCode.Events);

        await session.SaveChangesAsync();
        await RefreshDataAsync();
    }

    public async Task RefreshDataAsync()
    {
        await Beheer.RefreshDataAsync();
        await Publiek.RefreshDataAsync();
    }

    private Action<IWebHostBuilder> ConfigureForTesting(IConfigurationRoot configuration)
    {
        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());
            b.UseConfiguration(configuration);
        };
    }

    public async Task<IDocumentSession> DocumentSession()
    {
        IDocumentSession? session = null;

        try
        {
            session = Beheer.Host.DocumentStore().LightweightSession();

            session.SetHeader(MetadataHeaderNames.Initiator, MetadataInitiator);
            session.SetHeader(MetadataHeaderNames.Tijdstip, MetadataTijdstip);
            session.CorrelationId = Guid.NewGuid().ToString();

            return session;
        }
        catch
        {
            await session.DisposeAsync();

            throw;
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

    public async Task DisposeAsync()
    {
    }
}
