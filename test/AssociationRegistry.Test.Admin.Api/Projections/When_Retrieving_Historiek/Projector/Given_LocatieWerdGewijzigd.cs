namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;

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
public class Given_LocatieWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdGewijzigd = fixture.Create<TestEvent<LocatieWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(locatieWerdGewijzigd, doc);

        var naam = string.IsNullOrEmpty(locatieWerdGewijzigd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdGewijzigd.Data.Locatie.Naam}' ";

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"'{locatieWerdGewijzigd.Data.Locatie.Locatietype}' locatie {naam}werd gewijzigd.",
                nameof(LocatieWerdGewijzigd),
                locatieWerdGewijzigd.Data.Locatie,
                locatieWerdGewijzigd.Initiator,
                locatieWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
