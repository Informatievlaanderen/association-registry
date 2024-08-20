namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;

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
public class Given_LocatieWerdVerwijderd
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdVerwijderd = fixture.Create<TestEvent<LocatieWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(locatieWerdVerwijderd, doc);

        var naam = string.IsNullOrEmpty(locatieWerdVerwijderd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdVerwijderd.Data.Locatie.Naam}' ";

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"'{locatieWerdVerwijderd.Data.Locatie.Locatietype}' locatie {naam}werd verwijderd.",
                nameof(LocatieWerdVerwijderd),
                locatieWerdVerwijderd.Data.Locatie,
                locatieWerdVerwijderd.Initiator,
                locatieWerdVerwijderd.Tijdstip.ToZuluTime()));
    }
}
