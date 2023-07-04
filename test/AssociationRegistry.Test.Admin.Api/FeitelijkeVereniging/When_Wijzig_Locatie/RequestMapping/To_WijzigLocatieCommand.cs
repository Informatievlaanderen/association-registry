namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_WijzigLocatieCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<WijzigLocatieRequest>();

        var vCode = fixture.Create<VCode>();
        var locatieId = fixture.Create<int>();
        var command = request.ToCommand(vCode, locatieId);

        command.VCode.Should().Be(vCode);
        command.Locatie.LocatieId.Should().Be(locatieId);
        command.Locatie.Locatietype.Should().Be(Locatietype.Parse(request.Locatie.Locatietype));
        command.Locatie.Naam.Should().Be(request.Locatie.Naam);
        command.Locatie.IsPrimair.Should().Be(request.Locatie.IsPrimair);
        command.Locatie.Adres!.Straatnaam.Should().Be(request.Locatie.Adres!.Straatnaam);
        command.Locatie.Adres.Huisnummer.Should().Be(request.Locatie.Adres.Huisnummer);
        command.Locatie.Adres.Busnummer.Should().Be(request.Locatie.Adres.Busnummer);
        command.Locatie.Adres.Postcode.Should().Be(request.Locatie.Adres.Postcode);
        command.Locatie.Adres.Gemeente.Should().Be(request.Locatie.Adres.Gemeente);
        command.Locatie.Adres.Land.Should().Be(request.Locatie.Adres.Land);
        command.Locatie.AdresId!.Adresbron.Should().BeEquivalentTo(Adresbron.Parse(request.Locatie.AdresId!.Broncode));
        command.Locatie.AdresId.Bronwaarde.Should().BeEquivalentTo(request.Locatie.AdresId.Bronwaarde);
    }
}
