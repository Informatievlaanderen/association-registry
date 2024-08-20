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
public class Given_LocatieWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(locatieWerdToegevoegd, doc);

        var naam = string.IsNullOrEmpty(locatieWerdToegevoegd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdToegevoegd.Data.Locatie.Naam}' ";

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"'{locatieWerdToegevoegd.Data.Locatie.Locatietype}' locatie {naam}werd toegevoegd.",
                nameof(LocatieWerdToegevoegd),
                locatieWerdToegevoegd.Data.Locatie,
                locatieWerdToegevoegd.Initiator,
                locatieWerdToegevoegd.Tijdstip.ToZuluTime()));
    }
}
