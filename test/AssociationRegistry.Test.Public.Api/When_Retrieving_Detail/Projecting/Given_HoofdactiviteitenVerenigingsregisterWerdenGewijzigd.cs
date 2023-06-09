namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using Events;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_HoofdactiviteitenVerenigingsregisterWerdenGewijzigd
{
    [Fact]
    public void Then_it_replaces_the_hoofdactiviteitenVerenigingsLoket()
    {
        var fixture = new Fixture().CustomizeAll();
        var hoofactiviteitenVerenigingloketWerdenGewijzigd = fixture.Create<TestEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>();
        var projector = new PubliekVerenigingDetailProjection();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(hoofactiviteitenVerenigingloketWerdenGewijzigd, doc);

        doc.HoofdactiviteitenVerenigingsloket.Should()
            .BeEquivalentTo(hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket);
    }
}
