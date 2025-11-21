namespace AssociationRegistry.Test.When_Checking_If_AggregateExists;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using DecentraalBeheer.Vereniging;
using EventStore.ConflictResolution;
using FluentAssertions;
using Marten;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using Persoonsgegevens;
using Xunit;

public class Given_A_NonExisting_AggregateFixture : IAsyncLifetime
{
    public IDocumentStore Store { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_A_NonExisting_Aggregate));
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class Given_A_NonExisting_Aggregate : IClassFixture<Given_A_NonExisting_AggregateFixture>
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;
    private readonly IDocumentStore _documentStore;
    public Given_A_NonExisting_Aggregate(Given_A_NonExisting_AggregateFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _documentStore = fixture.Store;
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

        await using var session = _documentStore.LightweightSession();
        var eventStore = new EventStore(session, _conflictResolver, new PersoonsgegevensProcessor(new PersoonsgegevensEventTransformers(), new VertegenwoordigerPersoonsgegevensRepository(session,new VertegenwoordigerPersoonsgegevensQuery(session)), NullLogger<PersoonsgegevensProcessor>.Instance), NullLogger<EventStore>.Instance);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var nonExistingVCode = _fixture.Create<VCode>();

        await eventStore.Save(verenigingWerdGeregistreerd.VCode, EventStore.ExpectedVersion.NewStream, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              (dynamic)verenigingWerdGeregistreerd);

        (await eventStore.Exists(nonExistingVCode)).Should().BeFalse();
    }
}
