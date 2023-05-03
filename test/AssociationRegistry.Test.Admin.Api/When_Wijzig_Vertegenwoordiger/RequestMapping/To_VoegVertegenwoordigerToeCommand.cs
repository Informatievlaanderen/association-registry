﻿namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.WijzigVertegenwoordiger;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_WijzigVertegenwoordigerCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<WijzigVertegenwoordigerRequest>();

        var vCode = fixture.Create<VCode>();
        var vertegenwoordigerId = fixture.Create<int>();
        var command = request.ToCommand(vCode, vertegenwoordigerId);


        command.VCode.Should().Be(vCode);
        command.Vertegenwoordiger.VeretegenwoordigerId.Should().Be(vertegenwoordigerId);
        command.Vertegenwoordiger.IsPrimair.Should().Be(request.Vertegenwoordiger.IsPrimair);
        command.Vertegenwoordiger.Roepnaam.Should().BeEquivalentTo(request.Vertegenwoordiger.Roepnaam);
        command.Vertegenwoordiger.Rol.Should().BeEquivalentTo(request.Vertegenwoordiger.Rol);
        command.Vertegenwoordiger.Email!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Email);
        command.Vertegenwoordiger.Telefoon!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Telefoon);
        command.Vertegenwoordiger.Mobiel!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Mobiel);
        command.Vertegenwoordiger.SocialMedia!.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.SocialMedia);
    }
}
