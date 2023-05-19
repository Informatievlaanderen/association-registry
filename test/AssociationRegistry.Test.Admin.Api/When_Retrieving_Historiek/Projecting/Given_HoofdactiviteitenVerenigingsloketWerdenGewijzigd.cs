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
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAll();
        var projection = new BeheerVerenigingHistoriekProjection();
        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd = fixture.Create<TestEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        projection.Apply(hoofdactiviteitenVerenigingsloketWerdenGewijzigd, doc);


        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                "Hoofdactiviteiten verenigingsloket werden gewijzigd.",
                nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data,
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Initiator,
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Tijdstip.ToBelgianDateAndTime()));
    }
}
