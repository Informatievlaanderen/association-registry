namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.Wegwijs;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithErkenningWerdGeschorst()
                .Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningRedenVanSchorsingWerdGecorrigeerd(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var geregistreerdDoorOvoCode = scenario
            .GetVerenigingState()
            .Erkenningen.Single(e => e.ErkenningId == erkenningId)
            .GeregistreerdDoor.OvoCode;

        var ctx = CorrigeerRedenSchorsingErkenningContext<CommandhandlerScenarioBase>
            .Given(scenario, _ => erkenningId)
            .WithCommand(cmd => cmd);

        var organisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub().WithGemachtigdeOrganisaties(
            geregistreerdDoorOvoCode,
            [ctx.Metadata.Initiator]
        );

        await ctx.WhenHandled(service: organisatieBevoegdheidService.Object);

        ctx.ShouldHaveSaved(
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(erkenningId, [ctx.Metadata.Initiator]),
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                ctx.Command.Erkenning.ErkenningId,
                ctx.Command.Erkenning.RedenSchorsing
            )
        );
    }
}
