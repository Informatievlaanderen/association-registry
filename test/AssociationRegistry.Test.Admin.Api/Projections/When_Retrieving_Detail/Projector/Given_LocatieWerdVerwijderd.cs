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
public class Given_LocatieWerdVerwijderd
{
    [Fact]
    public void Then_it_removes_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdVerwijderd = new TestEvent<LocatieWerdVerwijderd>(fixture.Create<LocatieWerdVerwijderd>());

        var teVerwijderenLocatie = fixture.Create<Locatie>();
        teVerwijderenLocatie.LocatieId = locatieWerdVerwijderd.Data.Locatie.LocatieId;

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(teVerwijderenLocatie).ToArray();

        BeheerVerenigingDetailProjector.Apply(locatieWerdVerwijderd, doc);

        doc.Locaties.Should().HaveCount(3);
        doc.Locaties.Should().NotContainEquivalentOf(teVerwijderenLocatie);
        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
