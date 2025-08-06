namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;

public class To_ZetSubtypeTerugNaarNietBepaaldCommand
{
    [Fact]
    public void With_Subtype_NietBepaald_Then_We_Get_A_ZetSubtypeTerugNaarNietBepaaldCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest()
        {
            Subtype = VerenigingssubtypeCode.NietBepaald.Code,
        };

        var vCode = fixture.Create<VCode>();
        var command = request.ToZetSubtypeTerugNaarNietBepaaldCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Should().BeOfType(typeof(ZetSubtypeTerugNaarNietBepaaldCommand));
    }
}
