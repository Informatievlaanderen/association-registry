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
