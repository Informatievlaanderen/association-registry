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
public class Given_StartdatumWerdGewijzigdInKbo_With_Null
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_with_null()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigdInKbo>>();

        startdatumWerdGewijzigd.Data = new StartdatumWerdGewijzigdInKbo(Startdatum: null);

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "In KBO werd de startdatum verwijderd.",
                nameof(StartdatumWerdGewijzigdInKbo),
                startdatumWerdGewijzigd.Data,
                startdatumWerdGewijzigd.Initiator,
                startdatumWerdGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
