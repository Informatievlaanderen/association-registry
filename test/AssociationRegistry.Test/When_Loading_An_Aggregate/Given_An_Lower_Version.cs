namespace AssociationRegistry.Test.When_Loading_An_Aggregate;

using AssociationRegistry.Framework;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using Events;
using EventStore;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
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
                new AddressMatchConflictResolutionStrategy(),
            },
            new IEventPostConflictResolutionStrategy[]
            {
                new AddressMatchConflictResolutionStrategy(),
            });
    }

    [Fact]
    public async Task Then_it_Throws_Exception()
    {
        var documentStore = await TestDocumentStoreFactory.Create(nameof(Given_An_Lower_Version));

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver, NullLogger<EventStore>.Instance);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<VCode>();

        await eventStore.Save(vCode, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, locatieWerdToegevoegd);

        await Assert.ThrowsAsync<UnexpectedAggregateVersionException>(() => eventStore.Load<VerenigingState>(vCode, expectedVersion: 1));
        documentStore.Dispose();
    }

    [Theory]
    [InlineData(typeof(AdresWerdOvergenomenUitAdressenregister))]
    [InlineData(typeof(AdresKonNietOvergenomenWordenUitAdressenregister))]
    [InlineData(typeof(AdresWerdNietGevondenInAdressenregister))]
    [InlineData(typeof(AdresNietUniekInAdressenregister))]
    [InlineData(typeof(AdresWerdGewijzigdInAdressenregister))]
    [InlineData(typeof(AdresWerdOntkoppeldVanAdressenregister))]
    [InlineData(typeof(LocatieDuplicaatWerdVerwijderdNaAdresMatch))]
    [InlineData(typeof(AdresHeeftGeenVerschillenMetAdressenregister))]
    public async Task With_No_Conflicting_Events_Then_it_Loads_The_Latest_Version(Type eventType)
    {
        var documentStore = await TestDocumentStoreFactory.Create(nameof(Given_An_Lower_Version));

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver, NullLogger<EventStore>.Instance);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        var @event = (IEvent)new SpecimenContext(_fixture).Resolve(eventType);

        var vCode = _fixture.Create<VCode>();

        await eventStore.Save(vCode, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, @event);

        var aggregate = await eventStore.Load<VerenigingState>(vCode, expectedVersion: 1);
        aggregate.Version.Should().Be(2);
        documentStore.Dispose();
    }
}
