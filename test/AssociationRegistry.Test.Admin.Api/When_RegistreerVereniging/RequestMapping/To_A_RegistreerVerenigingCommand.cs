namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using AutoFixture;
using FluentAssertions;
using Vereniging.RegistreerVereniging;
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
            out var contactgegevens,
            out var locaties,
            out var vertegenwoordigers,
            out var hoofdactiviteiten,
            out var skipDuplicateDetection);

        naam.Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        startdatum.Should().Be(request.Startdatum);
        kboNummber.Should().Be(request.KboNummer);
        contactgegevens[0].Should().BeEquivalentTo(
            new RegistreerVerenigingCommand.Contactgegeven(
                request.Contactgegevens[0].Type.ToString(),
                request.Contactgegevens[0].Waarde,
                request.Contactgegevens[0].Omschrijving,
                request.Contactgegevens[0].IsPrimair));
        locaties.Should().BeEquivalentTo(request.Locaties);
        vertegenwoordigers.Should().BeEquivalentTo(
            request.Vertegenwoordigers
                .Select(
                    v =>
                        new RegistreerVerenigingCommand.Vertegenwoordiger(
                            v.Insz!,
                            v.PrimairContactpersoon,
                            v.Roepnaam,
                            v.Rol,
                            v.Contactgegevens.Select(
                                c =>
                                    new RegistreerVerenigingCommand.Contactgegeven(
                                        c.Type.ToString(),
                                        c.Waarde,
                                        c.Omschrijving,
                                        c.IsPrimair)).ToArray())));

        hoofdactiviteiten.Should().BeEquivalentTo(request.HoofdactiviteitenVerenigingsloket);
        skipDuplicateDetection.Should().BeFalse();
    }
}
