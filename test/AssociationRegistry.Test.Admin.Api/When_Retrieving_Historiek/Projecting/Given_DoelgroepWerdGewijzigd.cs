namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

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
                doelgroepWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
