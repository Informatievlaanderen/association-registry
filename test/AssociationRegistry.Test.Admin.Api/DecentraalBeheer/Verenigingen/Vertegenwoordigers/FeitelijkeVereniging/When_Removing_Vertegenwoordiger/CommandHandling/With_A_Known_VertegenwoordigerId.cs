namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Events;
using Xunit;

public class With_A_Known_VertegenwoordigerId
{
    private readonly VerwijderVertegenwoordigerContext<FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario(),
            s => s.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId);

    public With_A_Known_VertegenwoordigerId()
    {
        _ctx.Handle(_ctx.CreateCommand()).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _ctx.AggregateSessionMock.ShouldHaveLoaded<Vereniging>(_ctx.Scenario.VCode);
    }

    [Fact]
    public void Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdVerwijderd(
                _ctx.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                _ctx.Scenario.VertegenwoordigerWerdToegevoegd.Insz,
                _ctx.Scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                _ctx.Scenario.VertegenwoordigerWerdToegevoegd.Achternaam
            )
        );
    }
}
