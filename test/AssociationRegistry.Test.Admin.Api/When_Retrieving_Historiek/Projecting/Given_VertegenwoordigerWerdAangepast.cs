namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordigerWerdGewijzigd = fixture.Create<TestEvent<VertegenwoordigerWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);


        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Vertegenwoordiger '{vertegenwoordigerWerdGewijzigd.Data.Voornaam} {vertegenwoordigerWerdGewijzigd.Data.Achternaam}' werd gewijzigd.",
                nameof(VertegenwoordigerWerdGewijzigd),
                vertegenwoordigerWerdGewijzigd.Data,
                vertegenwoordigerWerdGewijzigd.Initiator,
                vertegenwoordigerWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()));
    }
}
