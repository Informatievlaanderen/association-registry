namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using AssociationRegistry.Admin.Api.Events;
using Helpers;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class VerenigingDbFixture:IDisposable
{
    public static async Task<EventStore> CreateEventStore()
        => new(await CreateDocumentStore());

    public static async Task<DocumentStore> CreateDocumentStore()
    {
        var documentStore = DocumentStore.For(
            opts =>
            {
                opts.Connection(GetConnectionString());
                opts.Events.StreamIdentity = StreamIdentity.AsString;
            });
        await WaitFor.PostGreSQLToBecomeAvailable(documentStore, LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForPostgresTestLogger"));
        return documentStore;
    }

    private static string GetConnectionString()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        Console.WriteLine(currentDirectory);
        var builder = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var configurationRoot = builder.Build();
        var connectionString = configurationRoot
            .GetValue<string>("eventstore_connectionstring");
        return connectionString;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
