namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Overlapping_Datums
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario(),
            s => s.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
            s => s.ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Throws_ErkenningBestaatAl()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerdInHuidig.Startdatum.Value),
            EindDatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerdInHuidig.Einddatum.Value),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_ctx.Scenario.ErkenningWerdGeregistreerdInHuidig.Hernieuwingsdatum.Value),
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningBestaatAl, _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId));
    }
}
