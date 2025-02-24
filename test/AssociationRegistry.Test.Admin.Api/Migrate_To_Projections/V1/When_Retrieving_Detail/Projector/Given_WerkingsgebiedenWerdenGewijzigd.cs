namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
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
