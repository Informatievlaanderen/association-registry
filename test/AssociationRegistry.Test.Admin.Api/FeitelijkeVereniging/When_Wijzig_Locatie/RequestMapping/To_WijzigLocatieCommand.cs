namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using Framework;
using Vereniging;
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
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigLocatieRequest>();

        var vCode = fixture.Create<VCode>();
        var locatieId = fixture.Create<int>();
        var command = request.ToCommand(vCode, locatieId);

        command.VCode.Should().Be(vCode);
        command.TeWijzigenLocatie.LocatieId.Should().Be(locatieId);
        command.TeWijzigenLocatie.Locatietype.Should().Be(Locatietype.Parse(request.Locatie.Locatietype!));
        command.TeWijzigenLocatie.Naam.Should().Be(request.Locatie.Naam);
        command.TeWijzigenLocatie.IsPrimair.Should().Be(request.Locatie.IsPrimair);
        command.TeWijzigenLocatie.Adres!.Straatnaam.Should().Be(request.Locatie.Adres!.Straatnaam);
        command.TeWijzigenLocatie.Adres.Huisnummer.Should().Be(request.Locatie.Adres.Huisnummer);
        command.TeWijzigenLocatie.Adres.Busnummer.Should().Be(request.Locatie.Adres.Busnummer);
        command.TeWijzigenLocatie.Adres.Postcode.Should().Be(request.Locatie.Adres.Postcode);
        command.TeWijzigenLocatie.Adres.Gemeente.Should().Be(request.Locatie.Adres.Gemeente);
        command.TeWijzigenLocatie.Adres.Land.Should().Be(request.Locatie.Adres.Land);
        command.TeWijzigenLocatie.AdresId!.Adresbron.Should().BeEquivalentTo(Adresbron.Parse(request.Locatie.AdresId!.Broncode));
        command.TeWijzigenLocatie.AdresId.Bronwaarde.Should().BeEquivalentTo(request.Locatie.AdresId.Bronwaarde);
    }
}
