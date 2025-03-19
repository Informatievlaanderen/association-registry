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
public class To_VerfijnSubtypeNaarFeitelijkeVerenigingCommand
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
}
