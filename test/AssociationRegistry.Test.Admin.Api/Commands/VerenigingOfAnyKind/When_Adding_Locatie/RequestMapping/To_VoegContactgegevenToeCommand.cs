namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;
using AdresId = AssociationRegistry.Admin.Api.Verenigingen.Common.AdresId;

[UnitTest]
[Category("Mapping")]
public class To_VoegLocatieToeCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command_With_Adres()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId = null;
        request.Locatie.Adres = fixture.Create<Adres>();

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Locatie.Locatietype.Waarde.Should().Be(request.Locatie.Locatietype);
        command.Locatie.Naam.Should().Be(request.Locatie.Naam);
        command.Locatie.IsPrimair.Should().Be(request.Locatie.IsPrimair);
        command.Locatie.Adres!.Straatnaam.Should().Be(request.Locatie.Adres!.Straatnaam);
        command.Locatie.Adres!.Huisnummer.Should().Be(request.Locatie.Adres!.Huisnummer);
        command.Locatie.Adres!.Busnummer.Should().Be(request.Locatie.Adres!.Busnummer);
        command.Locatie.Adres!.Postcode.Should().Be(request.Locatie.Adres!.Postcode);
        command.Locatie.Adres!.Gemeente.Should().Be(request.Locatie.Adres!.Gemeente);
        command.Locatie.Adres!.Land.Should().Be(request.Locatie.Adres!.Land);
        command.Locatie.AdresId.Should().BeNull();
    }

    [Fact]
    public void Then_We_Get_A_Correct_Command_With_AdresId()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId = fixture.Create<AdresId>();
        request.Locatie.Adres = null;

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Locatie.Locatietype.Waarde.Should().Be(request.Locatie.Locatietype);
        command.Locatie.Naam.Should().Be(request.Locatie.Naam);
        command.Locatie.IsPrimair.Should().Be(request.Locatie.IsPrimair);
        command.Locatie.Adres.Should().BeNull();
        command.Locatie.AdresId!.Adresbron.Code.Should().Be(request.Locatie.AdresId!.Broncode);
        command.Locatie.AdresId!.Bronwaarde.Should().Be(request.Locatie.AdresId!.Bronwaarde);
    }
}
