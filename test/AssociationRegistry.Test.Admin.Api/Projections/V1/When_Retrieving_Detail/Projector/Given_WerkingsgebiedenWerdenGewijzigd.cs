namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_WerkingsgebiedenWerdenGewijzigd
{
    [Fact]
    public void Then_it_replaces_the_werkingsgebieden()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var werkingsgebiedenWerdenGewijzigd =
            fixture.Create<TestEvent<WerkingsgebiedenWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(werkingsgebiedenWerdenGewijzigd, doc);

        doc.Werkingsgebieden.Should()
           .BeEquivalentTo(werkingsgebiedenWerdenGewijzigd.Data.Werkingsgebieden);
    }
}
