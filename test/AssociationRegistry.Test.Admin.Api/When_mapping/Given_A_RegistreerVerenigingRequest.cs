namespace AssociationRegistry.Test.Admin.Api.When_mapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_A_RegistreerVerenigingRequest
{
    [Fact]
    public void Then_We_Get_A_WijzigBasisgegevensCommand()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<RegistreerVerenigingRequest>();

        var actual = request.ToRegistreerVerenigingCommand();

        actual.Deconstruct(
            out var naam,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var kboNummber,
            out var contactInfoLijst,
            out var locaties,
            out var vertegenwoordigers);

        naam.Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        startdatum.Should().Be(request.StartDatum);
        kboNummber.Should().Be(request.KboNummer);
        contactInfoLijst.Should().BeEquivalentTo(request.ContactInfoLijst);
        locaties.Should().BeEquivalentTo(request.Locaties);
        vertegenwoordigers.Should().BeEquivalentTo(request.Vertegenwoordigers);
    }
}
