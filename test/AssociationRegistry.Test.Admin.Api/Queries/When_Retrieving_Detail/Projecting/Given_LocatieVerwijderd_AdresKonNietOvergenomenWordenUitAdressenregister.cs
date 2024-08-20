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
public class Given_LocatieVerwijderd_AdresKonNietOvergenomenWordenUitAdressenregister
{
   [Fact]
    public void Then_It_Does_Not_Throw()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();
        var locatieWerdVerwijderd = new TestEvent<LocatieWerdVerwijderd>(
            fixture.Create<LocatieWerdVerwijderd>() with
            {
                Locatie = locatieWerdToegevoegd.Data.Locatie,
            });

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
        BeheerVerenigingDetailProjector.Apply(locatieWerdVerwijderd, doc);
        BeheerVerenigingDetailProjector.Apply(adresKonNietOvergenomenWorden, doc);

        doc.Locaties.Should().BeEmpty();
    }
}
