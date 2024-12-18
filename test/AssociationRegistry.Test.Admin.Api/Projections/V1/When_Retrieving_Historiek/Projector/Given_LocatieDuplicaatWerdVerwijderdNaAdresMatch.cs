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
                $"Locatie '{naam}' met ID {locatieDuplicaatWerdVerwijderd.Data.VerwijderdeLocatieId} werd verwijderd omdat de gegevens exact overeenkomen met locatie ID {locatieDuplicaatWerdVerwijderd.Data.BehoudenLocatieId}.",
                nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
                locatieDuplicaatWerdVerwijderd.Data,
                locatieDuplicaatWerdVerwijderd.Initiator,
                locatieDuplicaatWerdVerwijderd.Tijdstip.FormatAsZuluTime()));
    }
}
