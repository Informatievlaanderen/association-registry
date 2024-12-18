namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;
using WellknownFormats = AssociationRegistry.Admin.ProjectionHost.Constants.WellknownFormats;

[UnitTest]
public class Given_EinddatumWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<EinddatumWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdGestopt, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"De einddatum van de vereniging werd gewijzigd naar '{verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'.",
                nameof(EinddatumWerdGewijzigd),
                verenigingWerdGestopt.Data,
                verenigingWerdGestopt.Initiator,
                verenigingWerdGestopt.Tijdstip.FormatAsZuluTime()));
    }
}
