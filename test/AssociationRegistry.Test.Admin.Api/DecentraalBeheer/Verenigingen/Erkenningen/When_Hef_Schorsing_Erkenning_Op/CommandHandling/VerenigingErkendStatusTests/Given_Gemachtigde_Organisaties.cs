namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.VerenigingErkendStatusTests;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
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
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_SchorsingWerdOpgeheven(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = HefSchorsingErkenningOpContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var geregistreerdDoor = scenario
            .GetVerenigingState()
            .Erkenningen.Single(e => e.ErkenningId == erkenningId)
            .GeregistreerdDoor.OvoCode;

        var service = ctx.OrganisatieBevoegdheidService.WithGemachtigdeOrganisaties(
            geregistreerdDoor,
            [ctx.Metadata.Initiator]
        );

        ctx = await ctx.WithCommand(cmd => cmd).WhenHandled(service.Object);

        ctx.ShouldHaveSaved(
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(erkenningId, [ctx.Metadata.Initiator]),
            new SchorsingVanErkenningWerdOpgeheven(erkenningId, ErkenningStatus.Actief.Value),
            new VerenigingWerdErkend()
        );
    }
}
