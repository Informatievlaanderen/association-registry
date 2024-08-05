namespace AssociationRegistry.Test.Admin.Api.Given_Conflicting_Events_Occur;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;

public class When_AdresWerdGeheradresseerd
{
    private readonly Fixture _fixture;

    public When_AdresWerdGeheradresseerd()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task ThenItSavesTheLocation()
    {
        var documentStore = await TestDocumentStoreFactory.Create(nameof(When_AdresWerdGeheradresseerd));

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        var eventConflictResolver = new EventConflictResolver(Array.Empty<IEventPreConflictResolutionStrategy>(), new IEventPostConflictResolutionStrategy[]
        {
            new AddressMatchConflictResolutionStrategy(),
        });

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver, NullLogger<EventStore>.Instance);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var adresWerdGewijzigdInAdressenregister = _fixture.Create<AdresWerdGewijzigdInAdressenregister>();
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<string>();

        await eventStore.Save(vCode, session, new CommandMetadata("brol", Instant.MinValue, Guid.NewGuid(), null), CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, adresWerdGewijzigdInAdressenregister);

        var result = await eventStore.Save(vCode, session, new CommandMetadata("brol", Instant.MinValue, Guid.NewGuid(), 1), CancellationToken.None,
                              locatieWerdToegevoegd);

        var savedEvents = await session.Events.FetchStreamAsync(vCode);
        Assert.Equal(3, savedEvents.Count);
        result.Version.Should().Be(3);
        documentStore.Dispose();
    }
}
