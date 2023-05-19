namespace AssociationRegistry.Test.Public.Api.When_Searching.Projecting;

using Events;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
using Framework;
using AutoFixture;
using FluentAssertions;
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
        var projector = new ElasticEventProjection(new VerenigingBrolFeeder());

        var doc = fixture.Create<VerenigingDocument>();

        projector.Apply(hoofactiviteitenVerenigingloketWerdenGewijzigd, doc);

        doc.HoofdactiviteitenVerenigingsloket.Should()
            .BeEquivalentTo(hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket);
    }
}
