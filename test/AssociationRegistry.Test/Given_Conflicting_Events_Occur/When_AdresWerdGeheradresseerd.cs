namespace AssociationRegistry.Test.Given_Conflicting_Events_Occur;

using AssociationRegistry.EventStore;
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
using Xunit;

public class When_AdresWerdGeheradresseerd
{
    private readonly Fixture _fixture;

    public When_AdresWerdGeheradresseerd()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task ThenItSavesTheLocation(Type verenigingType)
    {
        var context = new SpecimenContext(_fixture);

        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(When_AdresWerdGeheradresseerd));

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        var eventConflictResolver = new EventConflictResolver(Array.Empty<IEventPreConflictResolutionStrategy>(),
                                                              new IEventPostConflictResolutionStrategy[]
                                                              {
                                                                  new AddressMatchConflictResolutionStrategy(),
                                                              });

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver, NullLogger<EventStore>.Instance);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var adresWerdGewijzigdInAdressenregister = _fixture.Create<AdresWerdGewijzigdInAdressenregister>();
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<string>();

        await eventStore.Save(vCode, EventStore.ExpectedVersion.NewStream, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              (dynamic)verenigingWerdGeregistreerd, adresWerdGewijzigdInAdressenregister);

        var result = await eventStore.Save(vCode, 2, session,
                                           new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid(), ExpectedVersion: 1),
                                           CancellationToken.None,
                                           locatieWerdToegevoegd);

        var savedEvents = await session.Events.FetchStreamAsync(vCode);
        Assert.Equal(expected: 3, savedEvents.Count);
        result.Version.Should().Be(3);
        documentStore.Dispose();
    }
}
