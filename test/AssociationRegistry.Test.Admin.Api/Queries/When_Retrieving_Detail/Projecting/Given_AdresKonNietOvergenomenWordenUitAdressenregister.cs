namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister
{
    [Fact]
    public void Then_It_Clears_AdresId_And_VerwijstNaar()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();

        var adresKonNietOvergenomenWorden = new TestEvent<AdresKonNietOvergenomenWordenUitAdressenregister>(
            fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
            });

        var doc = fixture.Create<BeheerVerenigingDetailDocument>() with
        {
            Locaties = Array.Empty<Locatie>(),
        };

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        BeheerVerenigingDetailProjector.Apply(adresKonNietOvergenomenWorden, doc);

        var locatie = doc.Locaties.Single(x => x.LocatieId == locatieWerdToegevoegd.Data.Locatie.LocatieId);
        locatie.AdresId.Should().BeNull();
        locatie.VerwijstNaar.Should().BeNull();
    }
}
