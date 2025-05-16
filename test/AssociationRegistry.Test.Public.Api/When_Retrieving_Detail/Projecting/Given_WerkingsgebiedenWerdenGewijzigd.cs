namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_WerkingsgebiedenWerdenGewijzigd
{
    [Fact]
    public void Then_it_replaces_the_werkingsgebieden()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var werkingsgebiedenWerdenGewijzigd = fixture.Create<TestEvent<WerkingsgebiedenWerdenGewijzigd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(werkingsgebiedenWerdenGewijzigd, doc);

        doc.Werkingsgebieden.Should()
           .BeEquivalentTo(werkingsgebiedenWerdenGewijzigd.Data.Werkingsgebieden);
    }
}
