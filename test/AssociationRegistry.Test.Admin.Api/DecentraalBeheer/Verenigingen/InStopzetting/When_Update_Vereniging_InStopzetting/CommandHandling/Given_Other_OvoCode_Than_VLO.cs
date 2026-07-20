namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AutoFixture;
using Builders;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Other_OvoCode_Than_VLO
{
    [Fact]
    public async ValueTask Then_It_Throws_TypeVerenigingNietVzerVoorInStopzetting()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VzerScenarioBuilder().Build();

        var otherOvoCodeThanVlo = fixture.Create<string>();

        var ctx = UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithInitiator(otherOvoCodeThanVlo)
            .WithCommand(cmd => cmd);

        var exception = await Assert.ThrowsAsync<OvoCodeIsNietToegelatenDezeActieUitTeVoeren>(async () =>
            await ctx.WhenHandled()
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.OvoCodeIsNietGemachtigdOmDezeActieUitTeVoeren, otherOvoCodeThanVlo));
    }
}
