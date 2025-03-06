namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch
{
    [Fact]
    public void Then_it_removes_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var locatieDuplicaatWerdVerwijderdNaAdresMatch =
            new TestEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>(fixture.Create<LocatieDuplicaatWerdVerwijderdNaAdresMatch>());

        var teVerwijderenLocatie = fixture.Create<Locatie>();
        teVerwijderenLocatie.LocatieId = locatieDuplicaatWerdVerwijderdNaAdresMatch.Data.VerwijderdeLocatieId;

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(teVerwijderenLocatie).ToArray();

        BeheerVerenigingDetailProjector.Apply(locatieDuplicaatWerdVerwijderdNaAdresMatch, doc);

        doc.Locaties.Should().HaveCount(3);
        doc.Locaties.Should().NotContainEquivalentOf(teVerwijderenLocatie);
        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
