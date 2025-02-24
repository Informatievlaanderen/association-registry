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
public class Given_RoepnaamWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var roepnaamWerdGewijzigd = fixture.Create<TestEvent<RoepnaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(roepnaamWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Roepnaam werd gewijzigd naar '{roepnaamWerdGewijzigd.Data.Roepnaam}'.",
                nameof(RoepnaamWerdGewijzigd),
                roepnaamWerdGewijzigd.Data,
                roepnaamWerdGewijzigd.Initiator,
                roepnaamWerdGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
