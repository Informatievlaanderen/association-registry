namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.When_SyncKbo.CommandHandling;

using AssociationRegistry.Framework;
using AssociationRegistry.Framework.EventMetadata;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

/// <summary>
/// Integration test using real database to verify CommandMetadata.AdditionalMetadata
/// flows through EventStore and gets persisted to Marten headers.
/// </summary>
public class With_EventMetadata_In_Database : MetadataIntegrationTestBase
{
    private readonly string _sourceFileName = "kbo_mutation_2024_12_31.xml";

    protected override string SchemaName => "test_metadata_integration";

    [Fact]
    public async Task Given_CommandMetadata_With_TraceId_And_SourceFile__Then_Headers_Are_In_Database()
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        VCode = scenario.VCode;

        var commandMetadata = CreateCommandMetadata(SourceFileMetadata.KboSync(_sourceFileName));

        await using (var session = DocumentStore!.LightweightSession())
        {
            var eventStore = CreateEventStore(session);
            await eventStore.SaveNew(VCode!.ToString(), commandMetadata, CancellationToken.None, scenario.Events().ToArray());
        }

        await using (var querySession = DocumentStore.QuerySession())
        {
            var events = await querySession.Events.FetchStreamAsync(VCode!.ToString());
            var evt = events.First();

            evt.Headers[MetadataHeaderNames.TraceId].Should().Be(TraceId);
            evt.Headers[MetadataHeaderNames.Source].Should().Be(SyncSources.KboSync);
            evt.Headers[MetadataHeaderNames.SourceFileName].Should().Be(_sourceFileName);
            evt.CorrelationId.Should().Be(CorrelationId.ToString());
        }
    }
}
