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
public class Given_KorteBeschrijvingWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var korteBeschrijvingWerdGewijzigd = fixture.Create<TestEvent<KorteBeschrijvingWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Korte beschrijving werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}'.",
                nameof(KorteBeschrijvingWerdGewijzigd),
                korteBeschrijvingWerdGewijzigd.Data,
                korteBeschrijvingWerdGewijzigd.Initiator,
                korteBeschrijvingWerdGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
