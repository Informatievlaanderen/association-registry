namespace AssociationRegistry.Test.Public.Api.Fixtures;

using System.Reflection;
using AssociationRegistry.Public.Api;
using Common.Database;
using Framework.Helpers;
using JasperFx.CommandLine;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

public class StaticPublicApiFixture : IDisposable
{
    private const string Identifier = "staticpublicapifixture";
    private const string RootDatabase = @"postgres";
    public HttpClient HttpClient { get; }
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    public IDocumentStore DocumentStore { get; }

    public StaticPublicApiFixture()
    {
        JasperFxEnvironment.AutoStartHost = true;
        var configuration = GetConfiguration();

        WaitFor
            .PostGreSQLToBecomeAvailable(
                new NullLogger<StaticPublicApiFixture>(),
                GetConnectionString(configuration, RootDatabase)
            )
            .GetAwaiter()
            .GetResult();

        DropDatabase(configuration);
        CreateDatabaseFromTemplate(configuration);
        PublicMartenSchemaHelper.ApplyPublicApiMartenSchemaChanges(configuration);
        PublicMartenSchemaHelper.ApplyPublicProjectionHostMartenSchemaChanges(configuration);

        _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseConfiguration(configuration);
        });

        HttpClient = _webApplicationFactory.CreateClient();
        DocumentStore = _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();
    }

    private static IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
                     .SetBasePath(GetRootDirectory())
                     .AddJsonFile(path: "appsettings.json", optional: true)
                     .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var configurationRoot = builder.Build();
        configurationRoot["PostgreSQLOptions:database"] = Identifier;
        configurationRoot["ElasticClientOptions:Indices:Verenigingen"] = Identifier;

        return configurationRoot;
    }

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)
            ?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
    }

    private static void CreateDatabaseFromTemplate(IConfiguration configuration)
    {
        DatabaseTemplateHelper.CreateDatabaseFromTemplate(
            configuration,
            configuration["PostgreSQLOptions:database"]!,
            new NullLogger<StaticPublicApiFixture>()
        );
    }

    private static void DropDatabase(IConfiguration configuration)
    {
        using var connection = new NpgsqlConnection(GetConnectionString(configuration, RootDatabase));
        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            cmd.CommandText +=
                $"DROP DATABASE IF EXISTS \"{configuration["PostgreSQLOptions:database"]}\" WITH (FORCE);";
            cmd.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private static string GetConnectionString(IConfiguration configurationRoot, string database) =>
        $"host={configurationRoot["PostgreSQLOptions:host"]};"
        + $"database={database};"
        + $"password={configurationRoot["PostgreSQLOptions:password"]};"
        + $"username={configurationRoot["PostgreSQLOptions:username"]}";

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DropDatabase(GetConfiguration());
        HttpClient.Dispose();
        _webApplicationFactory.Dispose();
    }
}
