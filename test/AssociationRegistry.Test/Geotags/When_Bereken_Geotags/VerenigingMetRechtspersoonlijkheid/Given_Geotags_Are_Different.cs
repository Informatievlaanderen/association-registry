namespace AssociationRegistry.Test.Geotags.When_Bereken_Geotags.VerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Geotags;
using Events.Factories;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

public class Given_Geotags_Are_Different
{
    [Fact]
    public async Task Then_One_GeotagsWerdenBepaaldEvent()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        var verenigingWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        var geotagsWerdenBepaald = fixture.Create<GeotagsWerdenBepaald>();

        var (geotagsService, geotags) = Faktory.New().GeotagsService.ReturnsRandomGeotags();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(verenigingWerdGeregistreerd)
               .Apply(geotagsWerdenBepaald));

        await vereniging.HerberekenGeotags(geotagsService.Object);

        vereniging.UncommittedEvents.Count().Should().Be(1);
        vereniging.UncommittedEvents.ToArray().ShouldCompare(new IEvent[]
        {
            EventFactory.GeotagsWerdenBepaald(VCode.Create(verenigingWerdGeregistreerd.VCode), GeotagsCollection.Hydrate(geotags))
        });
    }
}
