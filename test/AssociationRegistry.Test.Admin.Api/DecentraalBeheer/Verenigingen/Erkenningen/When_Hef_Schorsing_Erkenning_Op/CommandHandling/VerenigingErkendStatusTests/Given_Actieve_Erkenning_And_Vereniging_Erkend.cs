namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.VerenigingErkendStatusTests;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_Actieve_Erkenning_And_Vereniging_Erkend
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithVerenigingWerdErkend()
                .WithErkenningWerdGeschorst()
                .WithVerenigingWerdNietLangerErkend()
                .BuildForVzer();

            var (vmerErkenningId, vmr) = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithVerenigingWerdErkend()
                .WithErkenningWerdGeschorst()
                .WithVerenigingWerdNietLangerErkend()
                .BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmerErkenningId } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Saves_SchorsingVanErkenningWerdOpgeheven_VerenigingWerdErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new HefSchorsingErkenningOpContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        await ctx.Handle(ctx.HefSchorsingErkenningOpCommand);

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(erkenningId, ErkenningStatus.Actief.Value),
            new VerenigingWerdErkend()
        );
    }
}
