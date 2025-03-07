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
    public void With_Subtype_FeitelijkeVereniging_Then_We_Get_A_VerfijnSubtypeNaarFeitelijkeVerenigingCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Verenigingssubtype.FeitelijkeVereniging.Code,
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToVerfijnSubtypeNaarFeitelijkeVerenigingCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Should().BeOfType(typeof(VerfijnSubtypeNaarFeitelijkeVerenigingCommand));
    }

    [Fact]
    public void With_Subtype_NogNietBepaald_Then_We_Get_A_ZetSubtypeTerugNaarNogNietBepaaldCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Verenigingssubtype.NogNietBepaald.Code,
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToZetSubtypeTerugNaarNogNietBepaaldCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Should().BeOfType(typeof(ZetSubtypeTerugNaarNogNietBepaaldCommand));
    }

    [Fact]
    public void With_Subtype_Subvereniging_Then_We_Get_A_TeWijzigenSubtype_As_SubtypeData_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Verenigingssubtype.SubVereniging.Code,
            AndereVereniging = fixture.Create<VCode>(),
            Beschrijving = fixture.Create<string>(),
            Identificatie = fixture.Create<string>(),
        };

        var vCode = fixture.Create<VCode>();
        var naam = fixture.Create<string>();
        var command = request.ToWijzigSubtypeCommand(vCode, naam);
        command.Should().BeOfType(typeof(WijzigSubtypeCommand));

        command.VCode.Should().Be(vCode);
        command.SubtypeData.Beschrijving.Should().BeEquivalentTo(SubtypeBeschrijving.Create(request.Beschrijving));
        command.SubtypeData.Identificatie.Should().BeEquivalentTo(SubtypeIdentificatie.Create(request.Identificatie));
        command.SubtypeData.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.SubtypeData.AndereVerenigingNaam.Should().Be(naam);
    }
}
