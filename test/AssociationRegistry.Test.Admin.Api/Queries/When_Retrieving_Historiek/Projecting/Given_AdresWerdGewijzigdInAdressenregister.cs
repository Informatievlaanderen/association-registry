<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_AdresWerdGewijzigdInAdressenregister.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_AdresWerdGewijzigdInAdressenregister.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
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
                                                                         adresWerdGewijzigdInAdressenregister.Tijdstip
                                                                            .ToZuluTime()));
    }
}
