namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync;

using AssociationRegistry.EventStore.ConflictResolution;
using AssociationRegistry.Framework;
using AssociationRegistry.Framework.EventMetadata;
using AssociationRegistry.MartenDb.Store;
using AssociationRegistry.MartenDb.Transformers;
using AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;
using AssociationRegistry.Test.Common.Framework;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;

/// <summary>
/// Base class for metadata integration tests that use real PostgreSQL database.
/// Provides common setup, helper methods, and ensures consistent cleanup.
/// </summary>
public abstract class MetadataIntegrationTestBase : IAsyncLifetime
{
    protected IDocumentStore? DocumentStore { get; private set; }
    protected readonly string TraceId = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    protected readonly Guid CorrelationId = Guid.NewGuid();
    protected AssociationRegistry.DecentraalBeheer.Vereniging.VCode? VCode { get; set; }

    /// <summary>
    /// Override to specify the database schema name for this test.
    /// </summary>
    protected abstract string SchemaName { get; }

    public async ValueTask InitializeAsync()
    {
        DocumentStore = await TestDocumentStoreFactory.CreateAsync(SchemaName);
    }

    /// <summary>
    /// Creates an EventStore instance with standard configuration for testing.
    /// </summary>
    protected EventStore CreateEventStore(IDocumentSession session)
    {
        return new EventStore(
            session,
            new EventConflictResolver([], []),
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(session, new VertegenwoordigerPersoonsgegevensQuery(session)),
                new BankrekeningnummerPersoonsgegevensRepository(session, new BankrekeningnummerPersoonsgegevensQuery(session)),
                NullLogger<PersoonsgegevensProcessor>.Instance),
            NullLogger<EventStore>.Instance);
    }

    /// <summary>
    /// Creates CommandMetadata with trace ID and source file metadata.
    /// </summary>
    protected CommandMetadata CreateCommandMetadata(SourceFileMetadata sourceFileMetadata)
    {
        var additionalMetadata = new EventMetadataCollection()
            .WithTrace(TraceId)
            .WithSource(sourceFileMetadata);

        return new CommandMetadata(
            WellknownOvoNumbers.DigitaalVlaanderenOvoNumber,
            SystemClock.Instance.GetCurrentInstant(),
            CorrelationId,
            null,
            additionalMetadata);
    }

    public async ValueTask DisposeAsync()
    {
        if (DocumentStore != null)
        {
            await DocumentStore.Advanced.ResetAllData();
            DocumentStore.Dispose();
        }
    }
}
