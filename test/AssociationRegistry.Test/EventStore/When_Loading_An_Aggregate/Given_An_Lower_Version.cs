namespace AssociationRegistry.Test.When_Loading_An_Aggregate;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using DecentraalBeheer.Vereniging;
using EventStore.ConflictResolution;
using FluentAssertions;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using Persoonsgegevens;
using Xunit;


public class GivenAnLowerVersionFixture : IAsyncLifetime
{
    public IDocumentStore Store { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Lower_Version));
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class Given_An_Lower_Version: IClassFixture<GivenAnLowerVersionFixture>
{
    private readonly Fixture _fixture;
    private readonly EventConflictResolver _conflictResolver;
    private readonly IDocumentStore _documentStore;

    public Given_An_Lower_Version(GivenAnLowerVersionFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _documentStore = fixture.Store;

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

        await using var session = _documentStore.LightweightSession();

        var eventStore = new EventStore(session, _conflictResolver, new PersoonsgegevensProcessor(
                                            new PersoonsgegevensEventTransformers(),
                                            new VertegenwoordigerPersoonsgegevensRepository(session,new VertegenwoordigerPersoonsgegevensQuery(session)),
                                            new BankrekeningnummerPersoonsgegevensRepository(session, new BankrekeningnummerPersoonsgegevensQuery(session)),
                                            NullLogger<PersoonsgegevensProcessor>.Instance), NullLogger<EventStore>.Instance);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var locatieWerdToegevoegd = _fixture.Create<LocatieWerdToegevoegd>();
        var vCode = VCode.Create(verenigingWerdGeregistreerd.VCode);

        await eventStore.Save(vCode, EventStore.ExpectedVersion.NewStream, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              (dynamic)verenigingWerdGeregistreerd, locatieWerdToegevoegd);

        await Assert.ThrowsAsync<UnexpectedAggregateVersionException>(() => eventStore.Load<VerenigingState>(vCode, expectedVersion: 1));
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
    public async Task With_FeitelijkeVereniging_With_No_Conflicting_Events_Then_it_Loads_The_Latest_Version(Type eventType)
    {
        await using var session = _documentStore.LightweightSession();

        var eventStore = new EventStore(session, _conflictResolver, new PersoonsgegevensProcessor(
                                            new PersoonsgegevensEventTransformers(),
                                            new VertegenwoordigerPersoonsgegevensRepository(session,new VertegenwoordigerPersoonsgegevensQuery(session)),
                                            new BankrekeningnummerPersoonsgegevensRepository(session, new BankrekeningnummerPersoonsgegevensQuery(session)),
                                            NullLogger<PersoonsgegevensProcessor>.Instance), NullLogger<EventStore>.Instance);
        var feitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        var @event = (IEvent)new SpecimenContext(_fixture).Resolve(eventType);

        var vCode = _fixture.Create<VCode>();

        await eventStore.Save(vCode, EventStore.ExpectedVersion.NewStream, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              feitelijkeVerenigingWerdGeregistreerd, @event);

        var aggregate = await eventStore.Load<VerenigingState>(vCode, expectedVersion: 1);
        aggregate.Version.Should().Be(2);
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
    public async Task With_VerenigingZonderEigenRechtspersoonlijkheid_With_No_Conflicting_Events_Then_it_Loads_The_Latest_Version(Type eventType)
    {
        await using var session = _documentStore.LightweightSession();

        var eventStore = new EventStore(session, _conflictResolver, new PersoonsgegevensProcessor(
                                            new PersoonsgegevensEventTransformers(),
                                            new VertegenwoordigerPersoonsgegevensRepository(session,new VertegenwoordigerPersoonsgegevensQuery(session)),
                                            new BankrekeningnummerPersoonsgegevensRepository(session, new BankrekeningnummerPersoonsgegevensQuery(session)),
                                            NullLogger<PersoonsgegevensProcessor>.Instance), NullLogger<EventStore>.Instance);
        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        var @event = (IEvent)new SpecimenContext(_fixture).Resolve(eventType);

        var vCode = _fixture.Create<VCode>();

        await eventStore.Save(vCode, EventStore.ExpectedVersion.NewStream, new CommandMetadata(Initiator: "brol", Instant.MinValue, Guid.NewGuid()),
                              CancellationToken.None,
                              verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, @event);


        var aggregate = await eventStore.Load<VerenigingState>(vCode, expectedVersion: 1);
        aggregate.Version.Should().Be(2);
    }
}
