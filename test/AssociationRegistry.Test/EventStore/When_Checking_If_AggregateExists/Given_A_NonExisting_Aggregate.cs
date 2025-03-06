namespace AssociationRegistry.Test.When_Checking_If_AggregateExists;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[IntegrationTest]
public class Given_A_NonExisting_Aggregate
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;

    public Given_A_NonExisting_Aggregate()
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
    public async Task Then_Returns_False(Type verenigingType)
    {
        var context = new SpecimenContext(_fixture);

        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_A_NonExisting_Aggregate));


        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver, NullLogger<EventStore>.Instance);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var nonExistingVCode = _fixture.Create<VCode>();

        await eventStore.Save(verenigingWerdGeregistreerd.VCode, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              (dynamic)verenigingWerdGeregistreerd);

        (await eventStore.Exists(nonExistingVCode)).Should().BeFalse();

        await documentStore.DisposeAsync();
    }
}
