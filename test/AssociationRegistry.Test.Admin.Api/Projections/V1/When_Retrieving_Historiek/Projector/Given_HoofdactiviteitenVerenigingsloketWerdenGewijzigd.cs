namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            fixture.Create<TestEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(hoofdactiviteitenVerenigingsloketWerdenGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "Hoofdactiviteiten verenigingsloket werden gewijzigd.",
                nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data,
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Initiator,
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
