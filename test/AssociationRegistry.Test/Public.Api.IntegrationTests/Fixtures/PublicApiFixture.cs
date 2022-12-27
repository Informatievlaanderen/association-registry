﻿namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures
{
    using System.Reflection;
    using AssociationRegistry.Admin.Api.Events;
    using AssociationRegistry.Admin.Api.Infrastructure;
    using AssociationRegistry.Framework;
    using AssociationRegistry.Public.Api;
    using AssociationRegistry.Public.Api.Extensions;
    using AssociationRegistry.Public.ProjectionHost.Projections.Search;
    using Marten;
    using Marten.Events;
    using Marten.Events.Projections;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nest;
    using NodaTime.Extensions;
    using Npgsql;
    using Xunit;
    using Xunit.Sdk;
    using IEvent = AssociationRegistry.Framework.IEvent;
    using Program = Program;

    public class PublicApiFixture : IDisposable, IAsyncLifetime
    {
        private const string RootDatabase = @"postgres";
        private readonly string _identifier = "p_";
        private readonly IConfigurationRoot _configurationRoot;

        private readonly IElasticClient _elasticClient;
        private readonly IDocumentStore _documentStore;
        private readonly TestServer _testServer;
        private IServiceProvider _projectionServices;

        public HttpClient HttpClient { get; }

        private string VerenigingenIndexName
            => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

        protected PublicApiFixture(string identifier)
        {
            _identifier += identifier.ToLowerInvariant();
            _configurationRoot = GetConfiguration();

            CreateDatabase();

            _testServer = ConfigureApiTestServer();

            HttpClient = _testServer.CreateClient();
            _elasticClient = CreateElasticClient(_testServer);
            // _documentStore = ConfigureDocumentStore(_testServer);
            _projectionServices = RunProjectionHost();
            _documentStore = _projectionServices.GetRequiredService<IDocumentStore>();
            ConfigureBrolFeeder(_projectionServices);

            ConfigureElasticClient(_elasticClient, VerenigingenIndexName);
        }


        private IConfigurationRoot GetConfiguration()
        {
            var maybeRootDirectory = Directory
                .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
            if (maybeRootDirectory is not { } rootDirectory)
                throw new NullReferenceException("Root directory cannot be null");

            var builder = new ConfigurationBuilder()
                .SetBasePath(rootDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);
            var tempConfiguration = builder.Build();
            tempConfiguration["PostgreSQLOptions:database"] = _identifier;
            tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;
            return tempConfiguration;
        }

        private static void ConfigureBrolFeeder(IServiceProvider projectionServices)
            => projectionServices.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

        protected async Task AddEvent(string vCode, IEvent eventToAdd, CommandMetadata? metadata = null)
        {
            if (_documentStore is not { })
                throw new NullException("DocumentStore cannot be null when adding an event");

            if (_elasticClient is not { })
                throw new NullException("Elastic client cannot be null when adding an event");

            if (metadata is null)
                metadata = new CommandMetadata(vCode, new DateTime(2022, 1, 1).ToUniversalTime().ToInstant());

            var eventStore = new EventStore(_documentStore);
            await eventStore.Save(vCode, metadata, eventToAdd);

            var daemon = await _documentStore.BuildProjectionDaemonAsync();
            await daemon.StartAllShards();
            await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(20));

            // Make sure all documents are properly indexed
            await _elasticClient.Indices.RefreshAsync(Indices.All);
        }

        public async Task<string> Search(string uri)
        {
            var responseMessage = await GetResponseMessage(uri);
            return await responseMessage.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> GetResponseMessage(string uri)
        {
            if (HttpClient is null)
                throw new NullReferenceException("HttpClient needs to be set before performing a get");

            return await HttpClient.GetAsync(uri);
        }

        private TestServer ConfigureApiTestServer()
        {
            IWebHostBuilder hostBuilder = new WebHostBuilder();
            hostBuilder.UseConfiguration(_configurationRoot);
            hostBuilder.UseStartup<Startup>();
            hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
            hostBuilder.UseTestServer();
            return new TestServer(hostBuilder);
        }

        private IServiceProvider RunProjectionHost()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder.UseContentRoot(Directory.GetCurrentDirectory());
                        builder.UseSetting("PostgreSQLOptions:database", _identifier);
                        builder.UseSetting("ElasticClientOptions:Indices:Verenigingen", _identifier);
                    });

            return webApplicationFactory.Services;
        }

        private static IElasticClient CreateElasticClient(TestServer testServer)
            => (IElasticClient)testServer.Services.GetRequiredService(typeof(ElasticClient));

        private static void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
        {
            if (client.Indices.Exists(verenigingenIndexName).Exists)
                client.Indices.Delete(verenigingenIndexName);

            client.Indices.CreateVerenigingIndex(verenigingenIndexName);

            client.Indices.Refresh(Indices.All);
        }

        private DocumentStore CreateDatabase()
        {
            return DocumentStore.For(
                opts =>
                {
                    var connectionString = GetConnectionString(_configurationRoot, _configurationRoot["PostgreSQLOptions:database"]);
                    var rootConnectionString = GetConnectionString(_configurationRoot, RootDatabase);
                    opts.Connection(connectionString);
                    opts.CreateDatabasesForTenants(
                        c =>
                        {
                            c.MaintenanceDatabase(rootConnectionString);
                            c.ForTenant()
                                .CheckAgainstPgDatabase()
                                .WithOwner(_configurationRoot["PostgreSQLOptions:username"]);
                        });
                    opts.Events.StreamIdentity = StreamIdentity.AsString;
                });
        }

        private void DropDatabase()
        {
            using var connection = new NpgsqlConnection(GetConnectionString(_configurationRoot, RootDatabase));
            using var cmd = connection.CreateCommand();
            try
            {
                connection.Open();
                // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
                cmd.CommandText += $"DROP DATABASE IF EXISTS {_configurationRoot["PostgreSQLOptions:database"]} WITH (FORCE);";
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            HttpClient.Dispose();
            _testServer.Dispose();
            _documentStore.Dispose();

            DropDatabase();

            _elasticClient.Indices.Delete(VerenigingenIndexName);
            _elasticClient.Indices.Refresh(Indices.All);
        }

        public virtual Task InitializeAsync()
            => Task.CompletedTask;

        public virtual Task DisposeAsync()
            => Task.CompletedTask;
    }
}
