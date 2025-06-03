namespace AssociationRegistry.Test.Geotags.When_Bereken_Geotags.VerenigingOfAnyKind;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Geotags;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Geotags_Are_The_Same
{
    [Fact]
    public async Task Then_No_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();

        var verenigingWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        var geotagsWerdenBepaald = fixture.Create<GeotagsWerdenBepaald>();

        var (geotagsService, geotags) = Faktory.New().GeotagsService.RetunsGeotags(GeotagsCollection.Hydrate(geotagsWerdenBepaald.Geotags));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(verenigingWerdGeregistreerd)
               .Apply(geotagsWerdenBepaald));

        await vereniging.HerberekenGeotags(geotagsService.Object);

        vereniging.UncommittedEvents.Count().Should().Be(0);
    }
}
