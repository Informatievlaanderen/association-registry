namespace AssociationRegistry.Test.Given_Conflicting_Events_Occur;

using AssociationRegistry.Framework;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Vereniging;
using Xunit;

public class When_Loading_Events
{
    private readonly Fixture _fixture;

    public When_Loading_Events()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_It_Loads_The_Event(Type verenigingType)
    {
        var context = new SpecimenContext(_fixture);

        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(When_Loading_Events));

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        var eventConflictResolver = new EventConflictResolver([new AddressMatchConflictResolutionStrategy(),], []);

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, eventConflictResolver, NullLogger<EventStore>.Instance);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var allowedTypeEvents = AddressMatchConflictResolutionStrategy
                               .AllowedTypes
                               .Select(type => (IEvent)context.Resolve(type))
                               .ToArray();

        var allEvents = new IEvent[]
                            { (dynamic)verenigingWerdGeregistreerd }
                       .Concat(allowedTypeEvents)
                       .ToArray();

        await eventStore.Save(verenigingWerdGeregistreerd.VCode, EventStore.ExpectedVersion.NewStream, session,
                              new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              allEvents);

        var result = await eventStore.Load<VerenigingState>(verenigingWerdGeregistreerd.VCode, 1);

        result.Version.Should().Be(allowedTypeEvents.Length + 1);
        await documentStore.DisposeAsync();
    }
}
