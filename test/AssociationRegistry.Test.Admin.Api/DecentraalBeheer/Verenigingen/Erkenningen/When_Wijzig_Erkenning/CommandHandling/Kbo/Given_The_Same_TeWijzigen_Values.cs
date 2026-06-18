namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Primitives;
using Xunit;

public class Given_The_Same_TeWijzigen_Values
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Should_Not_Have_Any_Saves()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value),
            EindDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value),
            HernieuwingsUrl = _ctx.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
