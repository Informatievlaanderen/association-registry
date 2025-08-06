namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class To_WijzigVertegenwoordigerCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigVertegenwoordigerRequest>();

        var vCode = fixture.Create<VCode>();
        var vertegenwoordigerId = fixture.Create<int>();
        var command = request.ToCommand(vCode, vertegenwoordigerId);

        command.VCode.Should().Be(vCode);
        command.Vertegenwoordiger.VertegenwoordigerId.Should().Be(vertegenwoordigerId);
        command.Vertegenwoordiger.IsPrimair.Should().Be(request.Vertegenwoordiger.IsPrimair);
        command.Vertegenwoordiger.Roepnaam.Should().BeEquivalentTo(request.Vertegenwoordiger.Roepnaam);
        command.Vertegenwoordiger.Rol.Should().BeEquivalentTo(request.Vertegenwoordiger.Rol);
        command.Vertegenwoordiger.Email!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Email);
        command.Vertegenwoordiger.Telefoon!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Telefoon);
        command.Vertegenwoordiger.Mobiel!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Mobiel);
        command.Vertegenwoordiger.SocialMedia!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.SocialMedia);
    }
}
