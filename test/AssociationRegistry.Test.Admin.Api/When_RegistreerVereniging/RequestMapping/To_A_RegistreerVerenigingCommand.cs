namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class To_A_RegistreerVerenigingCommand
{
    [Fact]
    public void Then_We_Get_A_RegistreerVerenigingCommand()
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
            out var vertegenwoordigers,
            out var hoofdactiviteiten,
            out var skipDuplicateDetection);

        naam.Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        startdatum.Should().Be(request.StartDatum);
        kboNummber.Should().Be(request.KboNummer);
        contactInfoLijst.Should().BeEquivalentTo(request.ContactInfoLijst);
        locaties.Should().BeEquivalentTo(request.Locaties);
        vertegenwoordigers.Should().BeEquivalentTo(request.Vertegenwoordigers);
        hoofdactiviteiten.Should().BeEquivalentTo(request.HoofdactiviteitenVerenigingsloket);
        skipDuplicateDetection.Should().BeFalse();
    }
}
