namespace AssociationRegistry.Test.Given_Conflicting_Events_Occur;

using AssociationRegistry.Framework;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using Events;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;

public class When_Saving_Events
{
    private readonly Fixture _fixture;

    public When_Saving_Events()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task ThenItSavesTheLocation(Type verenigingType)
    {
        var context = new SpecimenContext(_fixture);

        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(When_Saving_Events));

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        var eventConflictResolver = new EventConflictResolver(Array.Empty<IEventPreConflictResolutionStrategy>(),
                                                              new IEventPostConflictResolutionStrategy[]
                                                              {
                                                                  new AddressMatchConflictResolutionStrategy(),
                                                              });

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver, NullLogger<EventStore>.Instance);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        var vCode = _fixture.Create<string>();

        var allowedTypeEvents = AddressMatchConflictResolutionStrategy
                               .AllowedTypes
                               .Select(type => (IEvent)context.Resolve(type))
                               .ToArray();

        var allEvents = new IEvent[]
                            { (dynamic)verenigingWerdGeregistreerd }
                       .Concat(allowedTypeEvents)
                       .ToArray();

        await eventStore.Save(vCode, EventStore.ExpectedVersion.NewStream, session,
                              new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              allEvents);

        var result = await eventStore.Save(vCode, 2, session,
                                           new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid(), ExpectedVersion: 1),
                                           CancellationToken.None,
                                           locatieWerdToegevoegd);

        var savedEvents = await session.Events.FetchStreamAsync(vCode);
        Assert.Equal(expected: allowedTypeEvents.Length + 2, savedEvents.Count); // + 2 cause of registreer and voeg locatie toe event
        result.Version.Should().Be(allowedTypeEvents.Length + 2);
        await documentStore.DisposeAsync();
    }
}
