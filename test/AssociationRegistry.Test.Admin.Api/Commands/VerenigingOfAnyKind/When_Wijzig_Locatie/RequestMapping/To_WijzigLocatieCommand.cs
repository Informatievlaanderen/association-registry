namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestMapping;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common.Adres;
using AdresId = AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common.AdresId;

[UnitTest]
[Category("Mapping")]
public class To_WijzigLocatieCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command_With_Adres()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigLocatieRequest>();
        request.Locatie.AdresId = null;
        request.Locatie.Adres = fixture.Create<Adres>();

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
        command.TeWijzigenLocatie.Adres.Gemeente.Naam.Should().Be(request.Locatie.Adres.Gemeente);
        command.TeWijzigenLocatie.Adres.Land.Should().Be(request.Locatie.Adres.Land);
        command.TeWijzigenLocatie.AdresId.Should().BeNull();
    }

    [Fact]
    public void Then_We_Get_A_Correct_Command_With_AdresId()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigLocatieRequest>();
        request.Locatie.Adres = null;
        request.Locatie.AdresId = fixture.Create<AdresId>();

        var vCode = fixture.Create<VCode>();
        var locatieId = fixture.Create<int>();
        var command = request.ToCommand(vCode, locatieId);

        command.VCode.Should().Be(vCode);
        command.TeWijzigenLocatie.LocatieId.Should().Be(locatieId);
        command.TeWijzigenLocatie.Locatietype.Should().Be(Locatietype.Parse(request.Locatie.Locatietype!));
        command.TeWijzigenLocatie.Naam.Should().Be(request.Locatie.Naam);
        command.TeWijzigenLocatie.IsPrimair.Should().Be(request.Locatie.IsPrimair);
        command.TeWijzigenLocatie.Adres.Should().BeNull();
        command.TeWijzigenLocatie.AdresId!.Adresbron.Code.Should().Be(request.Locatie.AdresId!.Broncode);
        command.TeWijzigenLocatie.AdresId!.Bronwaarde.Should().Be(request.Locatie.AdresId!.Bronwaarde);
    }
}
