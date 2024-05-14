namespace AssociationRegistry.Test.Admin.Api.When_Loading_An_Aggregate;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Marten;
using Marten.Events;
using NodaTime;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Lower_Version
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;

    public Given_An_Lower_Version()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _conflictResolver = new EventConflictResolver(
            new IEventPreConflictResolutionStrategy[]
            {
                new AddressMatchConflictResolutionStrategy()
            },
            new IEventPostConflictResolutionStrategy[]
            {
                new AddressMatchConflictResolutionStrategy()
            });
    }

    [Fact(Skip = "")]
    public async Task Then_it_Throws_Exception()
    {
        //var documentStore = TestDocumentStoreFactory.Create();
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection(GetConnectionString());
            options.Events.StreamIdentity = StreamIdentity.AsString;
        });
        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<VCode>();

        await eventStore.Save(vCode, session, new CommandMetadata("brol", Instant.MinValue, Guid.NewGuid(), null), CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, locatieWerdToegevoegd);

        await Assert.ThrowsAsync<UnexpectedAggregateVersionException>(() => eventStore.Load<VerenigingState>(vCode, 1));
        documentStore.Dispose();
    }

    [Fact]
    public async Task With_No_Conflicting_Events_Then_it_Loads_The_Latest_Version()
    {
        //var documentStore = TestDocumentStoreFactory.Create();
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection(GetConnectionString());
            options.Events.StreamIdentity = StreamIdentity.AsString;
        });
        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var adresWerdOvergenomenUitAdressenregister = _fixture.Create<AdresWerdOvergenomenUitAdressenregister>();

        var vCode = _fixture.Create<VCode>();

        await eventStore.Save(vCode, session, new CommandMetadata("brol", Instant.MinValue, Guid.NewGuid(), null), CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, adresWerdOvergenomenUitAdressenregister);

        var aggregate = await eventStore.Load<VerenigingState>(vCode, 1);
        aggregate.Version.Should().Be(2);
        documentStore.Dispose();
    }

    private static string GetConnectionString()
        => $"host=127.0.0.1;" +
           $"database=verenigingsregister;" +
           $"password=root;" +
           $"username=root";
}
