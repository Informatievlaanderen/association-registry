namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_StartdatumWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAll();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(startdatumWerdGewijzigd, doc);


        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Startdatum werd gewijzigd naar '{startdatumWerdGewijzigd.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}'.",
                nameof(StartdatumWerdGewijzigd),
                startdatumWerdGewijzigd.Data,
                startdatumWerdGewijzigd.Initiator,
                startdatumWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()));
    }
}

[UnitTest]
public class Given_StartdatumWerdGewijzigd_With_Null
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_with_null()
    {
        var fixture = new Fixture().CustomizeAll();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigd>>();
        startdatumWerdGewijzigd.Data = fixture.Create<StartdatumWerdGewijzigd>() with
        {
            Startdatum = null,
        };

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(startdatumWerdGewijzigd, doc);


        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                "Startdatum werd verwijderd.",
                nameof(StartdatumWerdGewijzigd),
                startdatumWerdGewijzigd.Data,
                startdatumWerdGewijzigd.Initiator,
                startdatumWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()));
    }
}
