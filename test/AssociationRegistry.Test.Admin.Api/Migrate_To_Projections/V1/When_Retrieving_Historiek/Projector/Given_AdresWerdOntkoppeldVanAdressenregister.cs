namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AdresWerdOntkoppeldVanAdressenregister
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var adresWerdOntkoppeldVanAdressenregister = fixture.Create<TestEvent<AdresWerdOntkoppeldVanAdressenregister>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(adresWerdOntkoppeldVanAdressenregister, doc);

        doc.Gebeurtenissen.Should()
           .ContainEquivalentOf(new BeheerVerenigingHistoriekGebeurtenis(Beschrijving: "Adres werd ontkoppeld van het adressenregister.",
                                                                         nameof(AdresWerdOntkoppeldVanAdressenregister),
                                                                         adresWerdOntkoppeldVanAdressenregister.Data,
                                                                         adresWerdOntkoppeldVanAdressenregister.Initiator,
                                                                         adresWerdOntkoppeldVanAdressenregister.Tijdstip.FormatAsZuluTime()));
    }
}
