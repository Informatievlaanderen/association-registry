namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly VerloopErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario(),
            s => s.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
            s => s.ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_No_Saved_Event()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<ErkenningKanNietVerlopenWorden>(async () => await _ctx.Handle(command));

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
        exception.Message.Should().Be(
            string.Format(
                "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet verlopen worden.",
                _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.Startdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerdInVerleden.Einddatum.Value,
                ErkenningStatus.Verlopen.Value
            ));
    }
}
