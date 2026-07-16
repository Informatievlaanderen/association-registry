namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling;

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

            var scenario = new ErkenningScenarioBuilder(fixture).GivenActieveErkenning().Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningWerdVerwijderd(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = VerwijderErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

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
            new ErkenningWerdVerwijderd(erkenningId)
        );
    }
}
