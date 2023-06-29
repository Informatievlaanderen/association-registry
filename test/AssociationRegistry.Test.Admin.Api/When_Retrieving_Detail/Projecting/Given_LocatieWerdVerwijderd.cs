namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using Events;
using AutoFixture;
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
        var fixture = new Fixture().CustomizeAll();
        var locatieWerdVerwijderd = new TestEvent<LocatieWerdVerwijderd>(fixture.Create<LocatieWerdVerwijderd>());

        var teVerwidjerenLocatie = fixture.Create<BeheerVerenigingDetailDocument.Locatie>();
        teVerwidjerenLocatie.LocatieId = locatieWerdVerwijderd.Data.Locatie.LocatieId;

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(teVerwidjerenLocatie).ToArray();

        BeheerVerenigingDetailProjector.Apply(locatieWerdVerwijderd, doc);

        doc.Locaties.Should().HaveCount(3);
        doc.Locaties.Should().NotContainEquivalentOf(teVerwidjerenLocatie);
    }
}
