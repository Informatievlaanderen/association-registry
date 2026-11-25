namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Trek_Relatieaanvraag_In.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class To_TrekRelatieaanvraagInCommand
{
    private static readonly VCode IngetrokkenDoor = VCode.Create("V0001001");

    [Fact]
    public void Then_The_Command_Contains_The_Reden_And_The_Aanvrager_From_vr_namens()
    {
        var request = new TrekRelatieaanvraagInRequest { Reden = "De aanvraag werd per vergissing ingediend." };

        var command = request.ToCommand("RA0000001", IngetrokkenDoor);

        command
            .Should()
            .BeEquivalentTo(
                new TrekRelatieaanvraagInCommand(
                    RelatieaanvraagId.Create(1),
                    IngetrokkenDoor,
                    "De aanvraag werd per vergissing ingediend."
                )
            );
    }

    [Fact]
    public void Without_A_Reden_Then_It_Is_Empty()
    {
        var command = new TrekRelatieaanvraagInRequest().ToCommand("RA0000001", IngetrokkenDoor);

        command.Reden.Should().BeEmpty();
    }

    [Theory]
    [InlineData("ra0000001")]
    [InlineData(" RA0000001 ")]
    public void With_Another_Notation_Of_The_AanvraagId_Then_It_Is_Parsed(string aanvraagId)
    {
        var command = new TrekRelatieaanvraagInRequest().ToCommand(aanvraagId, IngetrokkenDoor);

        command.AanvraagId.Should().Be(RelatieaanvraagId.Create(1));
    }

    [Fact]
    public void With_An_Invalid_AanvraagId_Then_It_Throws_RelatieaanvraagIdIsOngeldig()
    {
        var toCommand = () => new TrekRelatieaanvraagInRequest().ToCommand("V0001001", IngetrokkenDoor);

        toCommand.Should().Throw<RelatieaanvraagIdIsOngeldig>();
    }
}
