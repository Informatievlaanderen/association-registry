namespace AssociationRegistry.Test.Admin.Api.Geotags.When_Herbereken_Geotags;

using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

public class Given_A_Vereniging_With_Unchanged_Geotags
{
    private static readonly GeotagsCollection Geotags = GeotagsCollection.Hydrate([new Geotag("some-geotag")]);

    public static readonly IEnumerable<object[]> Scenarios =
    [
        [new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario().WithGeotagsWerdenBepaald(Geotags)],
        [new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario().WithGeotagsWerdenBepaald(Geotags)],
    ];

    [Theory]
    [MemberData(nameof(Scenarios))]
    public async ValueTask Then_No_GeotagsWerdenBepaald_Event_Is_Saved(CommandhandlerScenarioBase scenario)
    {
        var ctx = new HerberekenGeotagsContext(scenario);
        ctx.SetupCalculateGeotags(Geotags);

        await ctx.Handle();

        ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
