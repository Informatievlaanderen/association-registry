namespace AssociationRegistry.Test.When_saving_a_vereniging;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Locaties;
using VCodes;
using Vereniging;
using VerenigingsNamen;
using Vertegenwoordigers;
using AutoFixture;
using ContactGegevens;
using FluentAssertions;
using Framework;
using Hoofdactiviteiten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_New_Vereniging : IAsyncLifetime
{
    private EventStoreMock _eventStore;
    private VerenigingsRepository _repo;
    private VCode _vCode;
    private Vereniging _vereniging;
    private VerenigingsNaam _naam;

    public Given_A_New_Vereniging()
    {
        _eventStore = new EventStoreMock();

        _repo = new VerenigingsRepository(_eventStore);

        _vCode = VCode.Create(1001);
        _naam = new VerenigingsNaam("Vereniging 1");
        _vereniging = Vereniging.Registreer(_vCode, _naam, null, null, null, null, Array.Empty<Contactgegeven>(), LocatieLijst.Empty, VertegenwoordigersLijst.Empty, HoofdactiviteitenVerenigingsloketLijst.Empty);
    }

    public async Task InitializeAsync()
        => await _repo.Save(_vereniging, new Fixture().Create<CommandMetadata>());

    [Fact]
    public void Then_the_VerenigingWerdGeregistreerd_is_sent_correctly_to_the_EventStore()
    {
        _eventStore.Invocations.Should().HaveCount(1);
        var invocation = _eventStore.Invocations.Single();

        invocation.AggregateId.Should().Be(_vCode);

        var theEvent = (VerenigingWerdGeregistreerd)invocation.Events.Single();

        theEvent.VCode.Should().Be(_vCode);
        theEvent.Naam.Should().Be(_naam);
        theEvent.KorteNaam.Should().BeNull();
        theEvent.KorteBeschrijving.Should().BeNull();
        theEvent.KboNummer.Should().BeNull();
        theEvent.Startdatum.Should().BeNull();
    }


    public Task DisposeAsync()
        => Task.CompletedTask;
}
