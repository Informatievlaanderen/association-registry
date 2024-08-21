namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_NaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_naam()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var naamWerdGewijzigd = fixture.Create<TestEvent<NaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(naamWerdGewijzigd, doc);

        doc.Naam.Should().Be(naamWerdGewijzigd.Data.Naam);
    }
}
