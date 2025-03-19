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
    public void With_Subtype_Subvereniging_Then_We_Get_A_TeWijzigenSubtype_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Verenigingssubtype.Subvereniging.Code,
            AndereVereniging = fixture.Create<VCode>(),
            Beschrijving = fixture.Create<string>(),
            Identificatie = fixture.Create<string>(),
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToWijzigSubtypeCommand(vCode);
        command.Should().BeOfType(typeof(VerfijnSubtypeNaarSubverenigingCommand));

        command.VCode.Should().Be(vCode);
        command.SubverenigingVan.Beschrijving.Should().BeEquivalentTo(SubtypeBeschrijving.Create(request.Beschrijving));
        command.SubverenigingVan.Identificatie.Should().BeEquivalentTo(SubtypeIdentificatie.Create(request.Identificatie));
        command.SubverenigingVan.AndereVereniging.Should().Be(VCode.Create(request.AndereVereniging));
        command.SubverenigingVan.AndereVerenigingNaam.Should().BeEmpty();
    }

    [Fact]
    public void With_Some_Null_Properties_Then_We_Get_A_TeWijzigenSubtype_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = Verenigingssubtype.Subvereniging.Code,
            AndereVereniging = null,
            Beschrijving = null,
            Identificatie = null,
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToWijzigSubtypeCommand(vCode);
        command.Should().BeOfType(typeof(VerfijnSubtypeNaarSubverenigingCommand));

        command.VCode.Should().Be(vCode);
        command.SubverenigingVan.Beschrijving.Should().BeNull();
        command.SubverenigingVan.Identificatie.Should().BeNull();
        command.SubverenigingVan.AndereVereniging.Should().BeNull();
        command.SubverenigingVan.AndereVerenigingNaam.Should().BeEmpty();
    }
}
