namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.DecentraalBeheer.Subtype;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_WijzigSubtypeCommandTests
{
    [Fact]
    public void With_Subtype_FeitelijkeVereniging_Then_We_Get_A_TeWijzigenNaarFeitelijkeVereniging_As_SubtypeData_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Subtype.FeitelijkeVereniging.Code,
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode, null);

        command.VCode.Should().Be(vCode);
        command.SubtypeData.Subtype.Should().Be(Subtype.Parse(request.Subtype));
        command.SubtypeData.Should().BeOfType(typeof(WijzigSubtypeCommand.TeWijzigenNaarFeitelijkeVereniging));
    }

    [Fact]
    public void With_Subtype_NogNietBepaald_Then_We_Get_A_TerugTeZettenNaarNogNietBepaald_As_SubtypeData_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Subtype.NogNietBepaald.Code,
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode, null);

        command.VCode.Should().Be(vCode);
        command.SubtypeData.Subtype.Should().Be(Subtype.Parse(request.Subtype));
        command.SubtypeData.Should().BeOfType(typeof(WijzigSubtypeCommand.TerugTeZettenNaarNogNietBepaald));
    }

    [Fact]
    public void With_Subtype_Subvereniging_Then_We_Get_A_TeWijzigenSubtype_As_SubtypeData_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Subtype.SubVereniging.Code,
            AndereVereniging = fixture.Create<VCode>(),
            Beschrijving = fixture.Create<string>(),
            Identificatie = fixture.Create<string>(),
        };

        var vCode = fixture.Create<VCode>();
        var naam = fixture.Create<string>();
        var command = request.ToCommand(vCode, naam);
        command.SubtypeData.Should().BeOfType(typeof(WijzigSubtypeCommand.TeWijzigenSubtype));

        var teWijzigenSubtype = command.SubtypeData as WijzigSubtypeCommand.TeWijzigenSubtype;
        command.VCode.Should().Be(vCode);
        teWijzigenSubtype!.Subtype.Should().Be(Subtype.Parse(request.Subtype));
        teWijzigenSubtype.Beschrijving.Should().BeEquivalentTo(SubtypeBeschrijving.Create(request.Beschrijving));
        teWijzigenSubtype.Identificatie.Should().BeEquivalentTo(SubtypeIdentificatie.Create(request.Identificatie));
        teWijzigenSubtype.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        teWijzigenSubtype.AndereVerenigingNaam.Should().Be(naam);
    }
}
