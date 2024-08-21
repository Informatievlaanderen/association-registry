namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_A_WijzigMaatschappelijkeZetelCommand
{
    [Fact]
    public void Then_We_Get_A_WijzigBasisgegevensCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigMaatschappelijkeZetelRequest>();

        var actualVCode = fixture.Create<VCode>();
        var actualLocatieId = fixture.Create<int>();
        var actual = request.ToCommand(actualVCode, actualLocatieId);

        actual.TeWijzigenLocatie.Deconstruct(out var locatieId, out var isPrimair, out var naam);

        actual.VCode.Should().Be(actualVCode);
        locatieId.Should().Be(actualLocatieId);
        naam.Should().Be(request.Locatie.Naam);
        isPrimair.Should().Be(request.Locatie.IsPrimair);
    }
}
