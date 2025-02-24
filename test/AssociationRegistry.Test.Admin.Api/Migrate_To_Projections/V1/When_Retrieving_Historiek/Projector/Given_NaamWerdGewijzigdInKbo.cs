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
public class Given_NaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var naamWerdGewijzigdInKbo = fixture.Create<TestEvent<NaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(naamWerdGewijzigdInKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"In KBO werd de naam gewijzigd naar '{naamWerdGewijzigdInKbo.Data.Naam}'.",
                nameof(NaamWerdGewijzigdInKbo),
                naamWerdGewijzigdInKbo.Data,
                naamWerdGewijzigdInKbo.Initiator,
                naamWerdGewijzigdInKbo.Tijdstip.FormatAsZuluTime()));
    }
}
