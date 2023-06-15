namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
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
        var fixture = new Fixture().CustomizeAll();
        var verenigingWerdVerwijderdUitPubliekeDatastroom = fixture.Create<TestEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdVerwijderdUitPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();
        doc.DatumLaatsteAanpassing.Should().Be(verenigingWerdVerwijderdUitPubliekeDatastroom.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(verenigingWerdVerwijderdUitPubliekeDatastroom.Sequence, verenigingWerdVerwijderdUitPubliekeDatastroom.Version));}
}
