namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Koepels.WijzigKoepelgegevens;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;
using FluentAssertions;
using Xunit;

public class To_WijzigKoepelgegevensCommand
{
    private const string VCodeValue = "V0001001";

    [Fact]
    public void With_All_Fields_Then_The_Command_Contains_All_Fields()
    {
        var request = new WijzigKoepelgegevensRequest
        {
            Koepelstatus = Koepelstatus.Koepel.Value,
            PrivacyVerklaringGeaccepteerd = true,
        };

        var command = request.ToCommand(VCodeValue);

        command
            .Should()
            .BeEquivalentTo(
                new WijzigKoepelgegevensCommand(
                    VCode.Create(VCodeValue),
                    Koepelstatus.Koepel,
                    PrivacyVerklaringGeaccepteerd: true
                )
            );
    }

    [Fact]
    public void Without_Fields_Then_The_Command_Contains_Nulls()
    {
        var command = new WijzigKoepelgegevensRequest().ToCommand(VCodeValue);

        command.VCode.Should().Be(VCode.Create(VCodeValue));
        command.Koepelstatus.Should().BeNull();
        command.PrivacyVerklaringGeaccepteerd.Should().BeNull();
    }
}
