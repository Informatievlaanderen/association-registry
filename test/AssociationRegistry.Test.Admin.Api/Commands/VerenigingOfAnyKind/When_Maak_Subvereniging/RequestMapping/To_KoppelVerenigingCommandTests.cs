namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Maak_Subvereniging.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_KoppelVerenigingCommandTests
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegLidmaatschapToeRequest>();
        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Lidmaatschap.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.Lidmaatschap.Geldigheidsperiode.Van.Should().Be(new GeldigVan(request.Van));
        command.Lidmaatschap.Geldigheidsperiode.Tot.Should().Be(new GeldigTot(request.Tot));
        command.Lidmaatschap.Identificatie.Should().Be(request.Identificatie);
        command.Lidmaatschap.Beschrijving.Should().Be(request.Beschrijving);
    }
}
