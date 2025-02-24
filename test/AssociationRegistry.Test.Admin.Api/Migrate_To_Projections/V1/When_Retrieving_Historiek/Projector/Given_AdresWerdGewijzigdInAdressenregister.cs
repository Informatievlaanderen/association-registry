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
public class Given_AdresWerdGewijzigdInAdressenregister
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var adresWerdGewijzigdInAdressenregister = fixture.Create<TestEvent<AdresWerdGewijzigdInAdressenregister>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(adresWerdGewijzigdInAdressenregister, doc);

        doc.Gebeurtenissen.Should()
           .ContainEquivalentOf(new BeheerVerenigingHistoriekGebeurtenis(Beschrijving: "Adres werd gewijzigd in het adressenregister.",
                                                                         nameof(AdresWerdGewijzigdInAdressenregister),
                                                                         adresWerdGewijzigdInAdressenregister.Data,
                                                                         adresWerdGewijzigdInAdressenregister.Initiator,
                                                                         adresWerdGewijzigdInAdressenregister.Tijdstip.FormatAsZuluTime()));
    }
}
