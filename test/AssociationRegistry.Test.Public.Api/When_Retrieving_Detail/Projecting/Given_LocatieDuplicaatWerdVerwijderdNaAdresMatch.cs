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
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch
{
    [Fact]
    public void Then_it_removes_the_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var locatieWerdVerwijderd = fixture.Create<TestEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>>();

        var teVerwijderenLocatie = fixture.Create<PubliekVerenigingDetailDocument.Types.Locatie>();
        teVerwijderenLocatie.LocatieId = locatieWerdVerwijderd.Data.VerwijderdeLocatieId;

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        doc.Locaties = doc.Locaties.Append(
            teVerwijderenLocatie).ToArray();

        PubliekVerenigingDetailProjector.Apply(locatieWerdVerwijderd, doc);

        doc.Locaties.Should().NotContain(teVerwijderenLocatie);
        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
