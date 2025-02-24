namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingWerdIngeschrevenInPubliekeDatastroom
{
    [Fact]
    public void Then_it_sets_IsUitgeschrevenUitPubliekeDatastroom_to_false()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdToegevoegdAanPubliekeDatastroom = fixture.Create<TestEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdToegevoegdAanPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeFalse();
    }
}
