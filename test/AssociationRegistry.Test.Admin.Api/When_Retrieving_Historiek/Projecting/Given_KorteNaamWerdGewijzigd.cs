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
public class Given_KorteNaamWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAll();
        var korteNaamWerdGewijzigd = fixture.Create<TestEvent<KorteNaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(korteNaamWerdGewijzigd, doc);


        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.",
                nameof(KorteNaamWerdGewijzigd),
                korteNaamWerdGewijzigd.Data,
                korteNaamWerdGewijzigd.Initiator,
                korteNaamWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()));
    }
}
