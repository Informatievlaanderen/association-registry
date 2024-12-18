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

[UnitTest]
public class Given_VerenigingWerdVerwijderd
{
    [Fact]
    public void Then_it_adds_the_verwijderd_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdVerwijderd = fixture.Create<TestEvent<VerenigingWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdVerwijderd, doc);

        doc.Gebeurtenissen.Should().HaveCount(1);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "Deze vereniging werd verwijderd.",
                nameof(VerenigingWerdVerwijderd),
                new { verenigingWerdVerwijderd.Data.Reden },
                verenigingWerdVerwijderd.Initiator,
                verenigingWerdVerwijderd.Tijdstip.FormatAsZuluTime()));
    }
}
