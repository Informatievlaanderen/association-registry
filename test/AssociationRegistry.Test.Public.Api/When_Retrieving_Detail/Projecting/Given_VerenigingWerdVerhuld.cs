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
public class Given_VerenigingWerdToegevoegdAanPubliekeDatastroom
{
    [Fact]
    public void Then_it_sets_IsUitgeschrevenUitPubliekeDatastroom_to_false()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var verenigingWerdToegevoegdAanPubliekeDatastroom =
            new TestEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom>(fixture.Create<VerenigingWerdIngeschrevenInPubliekeDatastroom>());

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(verenigingWerdToegevoegdAanPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeFalse();
    }
}
