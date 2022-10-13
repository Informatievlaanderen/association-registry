namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using AssociationRegistry.Admin.Api.Events;
using Helpers;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IEventStore = AssociationRegistry.Admin.Api.Events.IEventStore;

public class VerenigingDbFixture:IDisposable
{
    public IDocumentStore DocumentStore { get; }
    public IEventStore EventStore { get; }

    public VerenigingDbFixture()
    {
        DocumentStore = CreateDocumentStore().GetAwaiter().GetResult();
        EventStore = new EventStore(DocumentStore);
    }

    private static async Task<DocumentStore> CreateDocumentStore()
    {
        var documentStore = Marten.DocumentStore.For(
            opts =>
            {
                opts.Connection(GetConnectionString());
                opts.Events.StreamIdentity = StreamIdentity.AsString;
            });
        await WaitFor.PostGreSQLToBecomeAvailable(documentStore, LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitFotPostgresTestLogger"));
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
