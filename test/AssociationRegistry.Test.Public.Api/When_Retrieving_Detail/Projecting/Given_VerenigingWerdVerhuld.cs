namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingWerdToegevoegdAanPubliekeDatastroom
{
    [Fact]
    public void Then_it_sets_IsUitgeschrevenUitPubliekeDatastroom_to_false()
    {
        var fixture = new Fixture().CustomizeAll();
        var verenigingWerdToegevoegdAanPubliekeDatastroom = new TestEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom>(fixture.Create<VerenigingWerdIngeschrevenInPubliekeDatastroom>());

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(verenigingWerdToegevoegdAanPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeFalse();
        doc.DatumLaatsteAanpassing.Should().Be(verenigingWerdToegevoegdAanPubliekeDatastroom.Tijdstip.ToBelgianDate());

    }
}
