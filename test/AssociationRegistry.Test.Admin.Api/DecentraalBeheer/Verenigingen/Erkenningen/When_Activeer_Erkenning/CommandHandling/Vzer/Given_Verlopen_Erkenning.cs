namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly ActiveerErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario(),
            s => s.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
            s => s.ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Saves_An_ErkenningWerdGeactiveerd_Event()
    {
        var command = _ctx.ActiveerErkenningCommand;

        var exception = await Assert.ThrowsAsync<ErkenningKanNietGeactiveerdWorden>(async () => await _ctx.Handle(command));

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
        exception.Message.Should().Be(
            string.Format(
                "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet geactiveerd worden.",
                _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.Startdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.Einddatum.Value,
                ErkenningStatus.Verlopen.Value
            ));
    }
}
