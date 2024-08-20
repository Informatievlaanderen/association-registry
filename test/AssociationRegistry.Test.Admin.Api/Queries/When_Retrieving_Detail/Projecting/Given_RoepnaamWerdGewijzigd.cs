namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_RoepnaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteBeschrijving()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var roepnaamWerdGewijzigd = fixture.Create<TestEvent<RoepnaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(roepnaamWerdGewijzigd, doc);

        doc.Roepnaam.Should().Be(roepnaamWerdGewijzigd.Data.Roepnaam);
    }
}
