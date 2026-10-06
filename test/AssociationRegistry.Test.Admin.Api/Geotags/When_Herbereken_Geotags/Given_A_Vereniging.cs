namespace AssociationRegistry.Test.Admin.Api.Geotags.When_Herbereken_Geotags;

using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Vereniging
{
    public static readonly IEnumerable<object[]> Scenarios =
    [
        [new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()],
        [new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario()],
    ];

    [Theory]
    [MemberData(nameof(Scenarios))]
    public async ValueTask With_No_Geotags_Initiated_Then_A_GeotagsWerdenBepaald_Event_Is_Saved(
        CommandhandlerScenarioBase scenario
    )
    {
        var ctx = new HerberekenGeotagsContext(scenario);
        var geotags = GeotagsCollection.Hydrate([new Geotag("some-geotag")]);
        ctx.SetupCalculateGeotags(geotags);

        await ctx.Handle();

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new GeotagsWerdenBepaald(
                ctx.Scenario.VCode!,
                geotags.Select(x => new Registratiedata.Geotag(x.Identificatie)).ToArray()
            )
        );
    }
}
