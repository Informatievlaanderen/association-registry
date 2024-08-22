namespace AssociationRegistry.Test.Admin.Api.Given_Conflicting_Events_Occur;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using EventStore;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;

public class When_AdresMatch_Occurred
{
    private readonly Fixture _fixture;

    public When_AdresMatch_Occurred()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task ThenItSavesTheLocation()
    {
        var documentStore = await TestDocumentStoreFactory.Create(nameof(When_AdresMatch_Occurred));

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        var eventConflictResolver = new EventConflictResolver(Array.Empty<IEventPreConflictResolutionStrategy>(),
                                                              new IEventPostConflictResolutionStrategy[]
                                                              {
                                                                  new AddressMatchConflictResolutionStrategy(),
                                                              });

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver, NullLogger<EventStore>.Instance);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var adresWerdOvergenomenUitAdressenregister = _fixture.Create<AdresWerdOvergenomenUitAdressenregister>();
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<string>();

        await eventStore.Save(vCode, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, adresWerdOvergenomenUitAdressenregister);

        var result = await eventStore.Save(vCode, session,
                                           new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid(), ExpectedVersion: 1),
                                           CancellationToken.None,
                                           locatieWerdToegevoegd);

        var savedEvents = await session.Events.FetchStreamAsync(vCode);
        Assert.Equal(expected: 3, savedEvents.Count);
        result.Version.Should().Be(3);
        documentStore.Dispose();
    }
}
