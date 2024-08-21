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
public class Given_KorteNaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteNaam()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var korteNaamWerdGewijzigd = fixture.Create<TestEvent<KorteNaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(korteNaamWerdGewijzigd, doc);

        doc.KorteNaam.Should().Be(korteNaamWerdGewijzigd.Data.KorteNaam);
    }
}
