﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class To_VoegVertegenwoordigerToeCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegVertegenwoordigerToeRequest>();

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Vertegenwoordiger.Insz.ToString().Should().BeEquivalentTo(request.Vertegenwoordiger.Insz);
        command.Vertegenwoordiger.Voornaam.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Voornaam);
        command.Vertegenwoordiger.Achternaam.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Achternaam);
        command.Vertegenwoordiger.IsPrimair.Should().Be(request.Vertegenwoordiger.IsPrimair);
        command.Vertegenwoordiger.Roepnaam.Should().BeEquivalentTo(request.Vertegenwoordiger.Roepnaam);
        command.Vertegenwoordiger.Rol.Should().BeEquivalentTo(request.Vertegenwoordiger.Rol);
        command.Vertegenwoordiger.Email.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Email);
        command.Vertegenwoordiger.Telefoon.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Telefoon);
        command.Vertegenwoordiger.Mobiel.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.Mobiel);
        command.Vertegenwoordiger.SocialMedia.Waarde.Should().BeEquivalentTo(request.Vertegenwoordiger.SocialMedia);
    }
}
