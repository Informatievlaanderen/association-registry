namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.BevestigRelatieaanvraag;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class To_BevestigRelatieaanvraagCommand
{
    private static readonly VCode BevestigdDoor = VCode.Create("V0001002");

    [Fact]
    public void Then_The_Command_Contains_The_Relatiegegevens_And_The_Ontvanger_From_vr_namens()
    {
        var request = new BevestigRelatieaanvraagRequest
        {
            Identificatie = "LID-1",
            Beschrijving = "Lid van de koepel",
        };

        var command = request.ToCommand("RA0000001", BevestigdDoor);

        command
            .Should()
            .BeEquivalentTo(
                new BevestigRelatieaanvraagCommand(
                    RelatieaanvraagId.Create(1),
                    BevestigdDoor,
                    Relatiegegevens.Create("LID-1", "Lid van de koepel")
                )
            );
    }

    [Fact]
    public void Without_Relatiegegevens_Then_They_Are_Empty()
    {
        var command = new BevestigRelatieaanvraagRequest().ToCommand("RA0000001", BevestigdDoor);

        command.Relatiegegevens.Should().Be(Relatiegegevens.Empty);
    }

    [Theory]
    [InlineData("ra0000001")]
    [InlineData(" RA0000001 ")]
    public void With_Another_Notation_Of_The_AanvraagId_Then_It_Is_Parsed(string aanvraagId)
    {
        var command = new BevestigRelatieaanvraagRequest().ToCommand(aanvraagId, BevestigdDoor);

        command.AanvraagId.Should().Be(RelatieaanvraagId.Create(1));
    }

    [Fact]
    public void With_An_Invalid_AanvraagId_Then_It_Throws_RelatieaanvraagIdIsOngeldig()
    {
        var toCommand = () => new BevestigRelatieaanvraagRequest().ToCommand("V0001001", BevestigdDoor);

        toCommand.Should().Throw<RelatieaanvraagIdIsOngeldig>();
    }
}
