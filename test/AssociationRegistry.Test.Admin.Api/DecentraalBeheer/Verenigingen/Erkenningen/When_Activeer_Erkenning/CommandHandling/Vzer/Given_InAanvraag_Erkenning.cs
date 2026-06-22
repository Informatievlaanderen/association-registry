namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_Erkenning_InAanvraag
{
    private readonly ActiveerErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario(),
            s => s.ErkenningWerdGeregistreerdInToekomst.ErkenningId,
            s => s.ErkenningWerdGeregistreerdInToekomst.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_No_Saved_Event()
    {
        var command = _ctx.ActiveerErkenningCommand;

        var exception = await Assert.ThrowsAsync<ErkenningKanNietGeactiveerdWorden>(async () => await _ctx.Handle(command));

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
        exception.Message.Should().Be(
            string.Format(
                "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet geactiveerd worden.",
                _ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.Startdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerdInToekomst.Einddatum.Value,
                ErkenningStatus.InAanvraag.Value
            ));
    }
}
