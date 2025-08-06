namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

using AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using Helpers;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Core;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Vereniging;
using Xunit;
using IEvent = Events.IEvent;

public abstract class MultiStreamTestFixture : IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly TestStreamCollection _internalStreamCollection = new();
    private readonly PostgreSqlOptionsSection _postgreSqlOptions;
    private readonly IConfigurationRoot _configuration;
    public IHost Host { get; }
    public IDocumentStore DocumentStore { get; }
    protected Fixture Fixture { get; }

    protected MultiStreamTestFixture()
    {
        Fixture = new Fixture().CustomizeAdminApi();

        _configuration = ConfigurationHelper.GetConfiguration();

        _postgreSqlOptions = _configuration
                            .GetSection(PostgreSqlOptionsSection.SectionName)
                            .Get<PostgreSqlOptionsSection>(); // In your tests, you would most likely use the IHost for your

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                        .ConfigureServices(services =>
                         {
                             var martenConfig = services.AddMarten(
                                                             serviceProvider =>
                                                             {
                                                                 var opts = new StoreOptions();

                                                                 opts.Connection(_postgreSqlOptions.GetConnectionString());

                                                                 opts.Events.StreamIdentity = StreamIdentity.AsString;

                                                                 opts.Events.MetadataConfig.EnableAll();

                                                                 opts.Projections.StaleSequenceThreshold = TimeSpan.FromSeconds(30);

                                                                 opts.Projections.Add(
                                                                     new LocatieZonderAdresMatchProjection(
                                                                         serviceProvider
                                                                            .GetRequiredService<
                                                                                 ILogger<LocatieZonderAdresMatchProjection>>()),
                                                                     ProjectionLifecycle.Async);

                                                                 opts.RegisterDocumentType<LocatieZonderAdresMatchDocument>();

                                                                 opts.Schema.For<LocatieZonderAdresMatchDocument>()
                                                                     .UseNumericRevisions(true)
                                                                     .UseOptimisticConcurrency(false);

                                                                 opts.GeneratedCodeMode = TypeLoadMode.Dynamic;

                                                                 opts.AutoCreateSchemaObjects = AutoCreate.All;

                                                                 return opts;
                                                             })

                                                         // Using Solo in tests will help it start up a little quicker
                                                        .AddAsyncDaemon(DaemonMode.Solo);

                             martenConfig.ApplyAllDatabaseChangesOnStartup();
                         })
                        .Build();

        DocumentStore = Host.Services.GetRequiredService<IDocumentStore>();
    }

    protected void Stream(string vCode, IReadOnlyCollection<IEvent> events) => _internalStreamCollection.Add(vCode, events.ToArray());

    public async ValueTask InitializeAsync()
    {
        await WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AdminApiFixture>(), GetConnectionString(_configuration, RootDatabase));

        RecreateDatabase();

        await Host.StartAsync();

        await using var session = DocumentStore.LightweightSession();

        foreach (var (vCode, events) in _internalStreamCollection)
        {
            session.Events.StartStream<VerenigingOfAnyKind>(vCode, events);
        }

        await session.SaveChangesAsync();

        await DocumentStore.WaitForNonStaleProjectionDataAsync(5.Seconds());
    }

    public async ValueTask DisposeAsync()
    {
        await Host.StopAsync();
    }

    private void RecreateDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString(_configuration, RootDatabase));

        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            cmd.CommandText += $"DROP DATABASE IF EXISTS \"{_configuration["PostgreSQLOptions:database"]}\" WITH (FORCE);";
            cmd.ExecuteNonQuery();
            cmd.CommandText += $"CREATE DATABASE {_postgreSqlOptions.Database} WITH OWNER = {_postgreSqlOptions.Username};";
            cmd.ExecuteNonQuery();
        }
        catch (PostgresException ex)
        {
            if (ex.MessageText != $"database \"{_postgreSqlOptions.Database!.ToLower()}\" already exists")
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
