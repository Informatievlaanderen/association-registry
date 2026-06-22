namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Overlapping_Datums
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario(),
            s => s.ErkenningWerdGeregistreerdInToekomst.ErkenningId,
            s => s.ErkenningWerdGeregistreerdInToekomst.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Throws_ErkenningBestaatAl()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.Startdatum.Value),
            EindDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.Einddatum.Value),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.Hernieuwingsdatum.Value),
        };
        var command = _ctx.WijzigErkenningCommand with
        {
            Erkenning = erkenning,
        };

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningBestaatAl, _ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId));
    }
}
