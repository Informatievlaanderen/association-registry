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
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieDuplicaatWerdVerwijderd = fixture.Create<TestEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(locatieDuplicaatWerdVerwijderd, doc);

        var naam = string.IsNullOrEmpty(locatieDuplicaatWerdVerwijderd.Data.LocatieNaam)
            ? string.Empty
            : locatieDuplicaatWerdVerwijderd.Data.LocatieNaam;

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Locatie '{naam}' met ID {locatieDuplicaatWerdVerwijderd.Data.VerwijderdeLocatieId} werd verwijderd na adresmatch wegens duplicaat met ID {locatieDuplicaatWerdVerwijderd.Data.BehoudenLocatieId}.",
                nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
                locatieDuplicaatWerdVerwijderd.Data,
                locatieDuplicaatWerdVerwijderd.Initiator,
                locatieDuplicaatWerdVerwijderd.Tijdstip.ToZuluTime()));
    }
}
