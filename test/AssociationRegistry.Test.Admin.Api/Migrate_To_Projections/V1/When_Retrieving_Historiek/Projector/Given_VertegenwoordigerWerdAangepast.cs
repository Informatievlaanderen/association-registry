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
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdGewijzigd = fixture.Create<TestEvent<VertegenwoordigerWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Vertegenwoordiger '{vertegenwoordigerWerdGewijzigd.Data.Voornaam} {vertegenwoordigerWerdGewijzigd.Data.Achternaam}' werd gewijzigd.",
                nameof(VertegenwoordigerWerdGewijzigd),
                vertegenwoordigerWerdGewijzigd.Data,
                vertegenwoordigerWerdGewijzigd.Initiator,
                vertegenwoordigerWerdGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
