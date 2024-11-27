namespace AssociationRegistry.Test.Projections.Framework;

using Alba;
using AssociationRegistry.Framework;
using Hosts.Configuration;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using Oakton;
using AdminProjectionHostProgram = Admin.ProjectionHost.Program;
using PublicProjectionHostProgram = Public.ProjectionHost.Program;

public class ProjectionContext : IProjectionContext, IAsyncLifetime
{
    public ProjectionContext()
    {
        MetadataTijdstip = InstantPattern.General.Format(new Instant());
    }

    private const string RootDatabase = @"postgres";
    public string MetadataInitiator => "metadata.Initiator";
    public string MetadataTijdstip { get; }
    public string? AuthCookie { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }
    public IElasticClient AdminElasticClient { get; private set; }
    public IAlbaHost PublicProjectionHost { get; private set; }
    public IElasticClient PublicElasticClient { get; private set; }
    public IDocumentSession Session { get; private set; }

    public async Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.v2.beheer.json").Build();

        //DropDatabase();
        EnsureDbExists(configuration);

        OaktonEnvironment.AutoStartHost = true;

        AdminProjectionHost =
            await AlbaHost.For<AdminProjectionHostProgram>(
                ConfigureForTesting(configuration));

        AdminElasticClient = AdminProjectionHost.Services.GetRequiredService<IElasticClient>();

        PublicProjectionHost =
            await AlbaHost.For<PublicProjectionHostProgram>(
                ConfigureForTesting(configuration));

        PublicElasticClient = PublicProjectionHost.Services.GetRequiredService<IElasticClient>();

        await AdminProjectionHost.DocumentStore().Advanced.ResetAllData();
        await PublicProjectionHost.DocumentStore().Advanced.ResetAllData();

        await AdminProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await PublicProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await AdminProjectionHost.ResumeAllDaemonsAsync();
        await PublicProjectionHost.ResumeAllDaemonsAsync();

        Session = AdminProjectionHost.DocumentStore().LightweightSession();
    }

    public async Task SaveAsync(EventsPerVCode[] events)
    {
        await using var session = await DocumentSession();

        foreach (var eventsPerVCode in events)
        {
            session.Events.Append(eventsPerVCode.VCode, eventsPerVCode.Events);
        }

        await session.SaveChangesAsync();

        WaitForNonStaleProjectionData();
        await AdminElasticClient.Indices.RefreshAsync();
        await PublicElasticClient.Indices.RefreshAsync();
    }

    public async Task WaitForDataRefreshAsync()
    {
        WaitForNonStaleProjectionData();
        await AdminElasticClient.Indices.RefreshAsync();
        await PublicElasticClient.Indices.RefreshAsync();
    }

    public async Task DisposeAsync()
    {
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

    private void WaitForNonStaleProjectionData()
    {
        Task.WaitAll(
            AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60)),
            PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60))
        );
    }

    public async Task<IDocumentSession> DocumentSession()
    {
        IDocumentSession? session = null;

        try
        {
            session = AdminProjectionHost.DocumentStore().LightweightSession();

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
}
