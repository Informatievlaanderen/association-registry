namespace AssociationRegistry.Test.When_Checking_If_AggregateExists;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using EventStore.ConflictResolution;
using FluentAssertions;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;

public class Given_An_Existing_Aggregate
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;

    public Given_An_Existing_Aggregate()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _conflictResolver = new EventConflictResolver(
            [
                new AddressMatchConflictResolutionStrategy(),
            ],
            [
                new AddressMatchConflictResolutionStrategy(),
            ]);
    }

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_it_Returns_True(Type verenigingType)
    {
        var context = new SpecimenContext(_fixture);

        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Existing_Aggregate));


        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver, NullLogger<EventStore>.Instance);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        await eventStore.Save(verenigingWerdGeregistreerd.VCode, EventStore.ExpectedVersion.NewStream, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              (dynamic)verenigingWerdGeregistreerd);

        (await eventStore.Exists(VCode.Create(verenigingWerdGeregistreerd.VCode))).Should().BeTrue();

        await documentStore.DisposeAsync();
    }
}
