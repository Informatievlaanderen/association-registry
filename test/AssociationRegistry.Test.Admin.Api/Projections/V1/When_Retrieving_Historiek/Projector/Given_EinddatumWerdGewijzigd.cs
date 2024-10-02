namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Constants;
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
                verenigingWerdGestopt.Tijdstip.ToZuluTime()));
    }
}
