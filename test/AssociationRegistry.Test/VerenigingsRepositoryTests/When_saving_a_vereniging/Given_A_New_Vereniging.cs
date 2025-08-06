namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_saving_a_vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using MartenDb.Store;
using Moq;
using Vereniging.Geotags;
using Xunit;

public class Given_A_New_Vereniging : IAsyncLifetime
{
    private readonly EventStoreMock _eventStore;
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;
    private readonly Vereniging _vereniging;
    private readonly VerenigingsNaam _naam;

    public Given_A_New_Vereniging()
    {
        _eventStore = new EventStoreMock();

        _repo = new VerenigingsRepository(eventStore: _eventStore);

        _vCode = VCode.Create(vCode: 1001);
        _naam = VerenigingsNaam.Create(naam: "Vereniging 1");

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            Naam: _naam,
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep: Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens: [],
            Locaties: [],
            Vertegenwoordigers: [],
            HoofdactiviteitenVerenigingsloket: [],
            Werkingsgebieden: []
        );

        _vereniging = Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            command,
            new StubVCodeService(_vCode),
            Mock.Of<IGeotagsService>(),
            new ClockStub(now: DateTime.Today))
                                .GetAwaiter().GetResult();
    }

    public async ValueTask InitializeAsync()
        => await _repo.Save(_vereniging, new Fixture().Create<CommandMetadata>());

    [Fact]
    public void Then_the_FeitelijkeVerenigingWerdGeregistreerd_is_sent_correctly_to_the_EventStore()
    {
        _eventStore.SaveInvocations.Should().HaveCount(1);
        var invocation = _eventStore.SaveInvocations.Single();

        invocation.AggregateId.Should().Be(_vCode);

        var theEvent = (VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)invocation.Events.First();

        theEvent.VCode.Should().Be(_vCode);
        theEvent.Naam.Should().Be(_naam);
        theEvent.KorteNaam.Should().BeEmpty();
        theEvent.KorteBeschrijving.Should().BeEmpty();
        theEvent.Startdatum.Should().BeNull();
    }

    public ValueTask DisposeAsync()
        => new ValueTask(Task.CompletedTask);
}
