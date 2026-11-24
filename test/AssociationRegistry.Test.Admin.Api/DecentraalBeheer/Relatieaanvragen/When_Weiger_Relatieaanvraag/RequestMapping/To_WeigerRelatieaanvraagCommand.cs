namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Weiger_Relatieaanvraag.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.WeigerRelatieaanvraag;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class To_WeigerRelatieaanvraagCommand
{
    private static readonly VCode GeweigerdDoor = VCode.Create("V0001002");

    [Fact]
    public void Then_The_Command_Contains_The_Reden_And_The_Ontvanger_From_vr_namens()
    {
        var request = new WeigerRelatieaanvraagRequest { Reden = "Wij zijn geen lid van deze koepel." };

        var command = request.ToCommand("RA0000001", GeweigerdDoor);

        command
            .Should()
            .BeEquivalentTo(
                new WeigerRelatieaanvraagCommand(
                    RelatieaanvraagId.Create(1),
                    GeweigerdDoor,
                    "Wij zijn geen lid van deze koepel."
                )
            );
    }

    [Fact]
    public void Without_A_Reden_Then_It_Is_Empty()
    {
        var command = new WeigerRelatieaanvraagRequest().ToCommand("RA0000001", GeweigerdDoor);

        command.Reden.Should().BeEmpty();
    }

    [Theory]
    [InlineData("ra0000001")]
    [InlineData(" RA0000001 ")]
    public void With_Another_Notation_Of_The_AanvraagId_Then_It_Is_Parsed(string aanvraagId)
    {
        var command = new WeigerRelatieaanvraagRequest().ToCommand(aanvraagId, GeweigerdDoor);

        command.AanvraagId.Should().Be(RelatieaanvraagId.Create(1));
    }

    [Fact]
    public void With_An_Invalid_AanvraagId_Then_It_Throws_RelatieaanvraagIdIsOngeldig()
    {
        var toCommand = () => new WeigerRelatieaanvraagRequest().ToCommand("V0001001", GeweigerdDoor);

        toCommand.Should().Throw<RelatieaanvraagIdIsOngeldig>();
    }
}
