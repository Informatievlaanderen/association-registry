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
public class Given_VerenigingWerdVerwijderdUitPubliekeDatastroom
{
    [Fact]
    public void Then_it_sets_IsUitgeschrevenUitPubliekeDatastroom_to_true()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var verenigingWerdVerwijderdUitPubliekeDatastroom =
            new TestEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>(
                fixture.Create<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>());

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(verenigingWerdVerwijderdUitPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();
    }
}
