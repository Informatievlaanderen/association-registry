namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport;

using Alba;
using AssociationRegistry.Framework;
using Common.Configuration;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using Oakton;
using Xunit;
using ProjectionHostProgram = AssociationRegistry.Admin.ProjectionHost.Program;

[CollectionDefinition(nameof(PowerBiExportContext))]
public class RegistreerVerenigingCollection : ICollectionFixture<PowerBiExportContext>
{ }

public class PowerBiExportContext : IAsyncLifetime
{
    private string? _dbName = "powerbiexportprojections";
    private IDocumentStore? _store;
    private const string RootDatabase = @"postgres";

    public string? AuthCookie { get; private set; }
    public IAlbaHost ProjectionHost { get; private set; }
    public IDocumentSession Session => _store.LightweightSession();

    public async Task InitializeAsync()
    {
        DropDatabase();
        EnsureDbExists(GetConfiguration());

        OaktonEnvironment.AutoStartHost = true;

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.v2.powerbi.json").Build();

        ProjectionHost = await AlbaHost.For<ProjectionHostProgram>(ConfigureForTesting(configuration, _dbName));

        _store = ProjectionHost.DocumentStore();
        await _store.Advanced.ResetAllData();

        await _store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await ProjectionHost.ResumeAllDaemonsAsync();

    }

    public async Task DisposeAsync()
    {
        await ProjectionHost.DisposeAsync();
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
             .UseSetting(key: $"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Schema)}", value: schema)
             .UseSetting(key: $"GrarOptions:Sqs:AddressMatchQueueName", value: schema.ToLowerInvariant())
             .UseSetting(key: $"ElasticClientOptions:Indices:Verenigingen", value: _dbName)
             .UseSetting(key: "PostgreSQLOptions:Database", _dbName);
        };
    }

    public async Task WaitForNonStaleProjectionDataAsync()
        => await ProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

    public async Task<IDocumentSession> DocumentSession()
    {
        IDocumentSession? session = null;

        try
        {
            session = ProjectionHost.DocumentStore().LightweightSession();

            session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
            session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
            session.CorrelationId = Guid.NewGuid().ToString();

            return session;
        }
        catch
        {
            await session.DisposeAsync();

            throw;
        }
    }

    private IConfigurationRoot GetConfiguration()
    {
        var tempConfiguration = ConfigurationHelper.GetConfiguration();
        tempConfiguration["PostgreSQLOptions:database"] = _dbName;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _dbName;

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
