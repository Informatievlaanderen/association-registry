namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_saving_a_vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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

        _repo = new VerenigingsRepository(_eventStore);

        _vCode = VCode.Create(1001);
        _naam = VerenigingsNaam.Create("Vereniging 1");

        _vereniging = Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            _vCode,
            _naam,
            korteNaam: null,
            korteBeschrijving: null,
            startDatum: null,
            Doelgroep.Null,
            uitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Array.Empty<Werkingsgebied>(),
            new ClockStub(DateTime.Today));
    }

    public async Task InitializeAsync()
        => await _repo.Save(_vereniging, new Fixture().Create<CommandMetadata>());

    [Fact]
    public void Then_the_FeitelijkeVerenigingWerdGeregistreerd_is_sent_correctly_to_the_EventStore()
    {
        _eventStore.SaveInvocations.Should().HaveCount(1);
        var invocation = _eventStore.SaveInvocations.Single();

        invocation.AggregateId.Should().Be(_vCode);

        var theEvent = (VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)invocation.Events.Single();

        theEvent.VCode.Should().Be(_vCode);
        theEvent.Naam.Should().Be(_naam);
        theEvent.KorteNaam.Should().BeEmpty();
        theEvent.KorteBeschrijving.Should().BeEmpty();
        theEvent.Startdatum.Should().BeNull();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
