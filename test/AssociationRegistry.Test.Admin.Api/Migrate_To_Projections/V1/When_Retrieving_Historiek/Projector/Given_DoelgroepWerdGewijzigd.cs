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
public class Given_DoelgroepWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var doelgroepWerdGewijzigd = fixture.Create<TestEvent<DoelgroepWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(doelgroepWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Doelgroep werd gewijzigd naar '{doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd} - {doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd}'.",
                nameof(DoelgroepWerdGewijzigd),
                doelgroepWerdGewijzigd.Data,
                doelgroepWerdGewijzigd.Initiator,
                doelgroepWerdGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
