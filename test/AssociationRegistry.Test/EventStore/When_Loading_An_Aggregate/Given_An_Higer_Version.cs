namespace AssociationRegistry.Test.When_Loading_An_Aggregate;

using AssociationRegistry.Framework;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using EventStore.ConflictResolution;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using Persoonsgegevens;
using Vereniging;
using Xunit;

public class Given_An_Higer_Version
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;

    public Given_An_Higer_Version()
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

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_it_Throws_Exception(Type verenigingType)
    {
        var context = new SpecimenContext(_fixture);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var vCode = VCode.Create(verenigingWerdGeregistreerd.VCode);

        var eventStore = await SetupEventStore(vCode, verenigingWerdGeregistreerd);

        await Assert.ThrowsAsync<UnexpectedAggregateVersionException>(() => eventStore.Load<VerenigingState>(vCode, expectedVersion: 999));
    }

    private async Task<EventStore> SetupEventStore(VCode vCode, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd verenigingWerdGeregistreerd)
    {
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Higer_Version));

        await using var session = documentStore.LightweightSession();
        var eventStore = new EventStore(documentStore, _conflictResolver, Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>(), NullLogger<EventStore>.Instance);
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();

        await eventStore.Save(vCode, EventStore.ExpectedVersion.NewStream, session, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              (dynamic)verenigingWerdGeregistreerd, locatieWerdToegevoegd);

        return eventStore;
    }
}
