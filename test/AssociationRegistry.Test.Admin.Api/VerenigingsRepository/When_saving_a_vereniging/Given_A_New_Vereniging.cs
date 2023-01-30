namespace AssociationRegistry.Test.Admin.Api.VerenigingsRepository.When_saving_a_vereniging;

using AssociationRegistry.EventStore;
using AutoFixture;
using ContactInfo;
using Events;
using FluentAssertions;
using global::AssociationRegistry.Framework;
using Locaties;
using VCodes;
using Vereniging;
using VerenigingsNamen;
using Vertegenwoordigers;
using Xunit;

public class Given_A_New_Vereniging
{
    [Fact]
    public async Task Then_the_verenigingcreatedevent_is_stored_in_the_EventStore()
    {
        var eventStore = new EventStoreMock();

        var repo = new VerenigingsRepository(eventStore);

        var vCode = VCode.Create(1001);
        var naam = new VerenigingsNaam("Vereniging 1");
        var vereniging = Vereniging.Registreer(vCode, naam, null, null, null, null, ContactLijst.Empty, LocatieLijst.Empty, VertegenwoordigersLijst.Empty, DateOnly.FromDateTime(DateTime.Today));

        await repo.Save(vereniging, new Fixture().Create<CommandMetadata>());
        eventStore.Invocations.Should().HaveCount(1);
        var invocation = eventStore.Invocations.Single();
        invocation.AggregateId.Should().Be(vCode);

        var theEvent = (VerenigingWerdGeregistreerd)invocation.Events.Single();

        theEvent.VCode.Should().Be("V0001001");
        theEvent.Naam.Should().Be("Vereniging 1");
        theEvent.KorteNaam.Should().BeNull();
        theEvent.KorteBeschrijving.Should().BeNull();
        theEvent.KboNummer.Should().BeNull();
        theEvent.Startdatum.Should().BeNull();
    }
}
