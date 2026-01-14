namespace AssociationRegistry.Test.When_EventStreamUnexpectedMaxEventIdException_Occured;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using EventStore.ConflictResolution;
using FluentAssertions;
using Marten;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using Persoonsgegevens;
using Xunit;

/// <summary>
/// Trying to save events after an adresmatch event is saved.
/// Because adresmatch events are async, this can result in a change between loading and saving events on a vereniging.
/// AddressMatchConflictResolutionStrategy allows saving events when this happens.
/// </summary>
public class Given_Saving_After_AdresMatch_Event
{
    private readonly Fixture _fixture;
    private readonly IDocumentSession _session;
    private readonly EventStore _sut;

    private readonly string _vCode;
    private readonly long _aggregateVersion = 1;

    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd _registrationEvent;

    public Given_Saving_After_AdresMatch_Event()
    {
        _fixture = new Fixture().CustomizeDomain();

        var documentStore = CreateDocumentStore();
        _session = documentStore.LightweightSession();

        _sut = CreateEventStore(_session);

        _registrationEvent = _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        _vCode = _registrationEvent.VCode;

        SeedEventStream();
    }

    [Fact]
    public async Task With_Lower_AggregateVersion_EventStreamUnexpectedMaxEventIdException_Then_Persoonsgegevens_Are_Saved()
    {
        // Act
        var registrationWithoutPersoonsgegevens =
            await _session.Events
                .QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>()
                .SingleAsync();

        var vertegenwoordigerGewijzigdWithoutPersoonsgegevens =
            await _session.Events
                .QueryRawEventDataOnly<VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens>()
                .SingleAsync();

        var vertegenwoordigerPersoonsgegevens =
            await _session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                .ToListAsync();

        // Assert
        vertegenwoordigerPersoonsgegevens.Should()
            .HaveCount(_registrationEvent.Vertegenwoordigers.Length + 1); // +1 from vertegenwoordigerWerdGewijzigd

        var expectedRefIds =
            registrationWithoutPersoonsgegevens.Vertegenwoordigers
                .Select(x => x.RefId)
                .Append(vertegenwoordigerGewijzigdWithoutPersoonsgegevens.RefId)
                .ToArray();

        vertegenwoordigerPersoonsgegevens.Select(x => x.RefId)
            .Should()
            .BeEquivalentTo(expectedRefIds);
    }

    private void SeedEventStream()
    {
        SaveRegistrationEvent();
        SaveAdresWerdOvergenomenUitAdressenregister();
        SaveVertegenwoordigerWerdGewijzigdForExistingRepresentative();
    }

    private void SaveRegistrationEvent()
    {
        _sut.SaveNew(
                _vCode,
                _fixture.Create<CommandMetadata>(),
                CancellationToken.None,
                [_registrationEvent])
            .GetAwaiter()
            .GetResult();
    }

    private void SaveAdresWerdOvergenomenUitAdressenregister()
    {
        var adresWerdOvergenomen = _fixture.Create<AdresWerdOvergenomenUitAdressenregister>();

        _sut.Save(
                _vCode,
                _aggregateVersion,
                _fixture.Create<CommandMetadata>(),
                CancellationToken.None,
                adresWerdOvergenomen)
            .GetAwaiter()
            .GetResult();
    }

    private void SaveVertegenwoordigerWerdGewijzigdForExistingRepresentative()
    {
        var existingVertegenwoordigerId = _registrationEvent.Vertegenwoordigers.First().VertegenwoordigerId;

        var vertegenwoordigerWerdGewijzigd = _fixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = existingVertegenwoordigerId,
        };

        _sut.Save(
                _vCode,
                _aggregateVersion,
                _fixture.Create<CommandMetadata>(),
                CancellationToken.None,
                vertegenwoordigerWerdGewijzigd)
            .GetAwaiter()
            .GetResult();
    }

    private static IDocumentStore CreateDocumentStore()
        => TestDocumentStoreFactory
            .CreateAsync(nameof(Given_Saving_After_AdresMatch_Event))
            .GetAwaiter()
            .GetResult();

    private static EventStore CreateEventStore(IDocumentSession session)
        => new(
            session,
            new EventConflictResolver(
                [new AddressMatchConflictResolutionStrategy()],
                [new AddressMatchConflictResolutionStrategy()]),
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(
                    session,
                    new VertegenwoordigerPersoonsgegevensQuery(session)),
                new NullLogger<PersoonsgegevensProcessor>()),
            NullLogger<EventStore>.Instance);
}
