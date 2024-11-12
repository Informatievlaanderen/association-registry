namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Formats;
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
