namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_NaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_naam()
    {
        var fixture = new Fixture().CustomizeAll();
        var naamWerdGewijzigd = fixture.Create<TestEvent<NaamWerdGewijzigd>>();
        var projector = new BeheerVerenigingDetailProjection();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        projector.Apply(naamWerdGewijzigd, doc);

        doc.Naam.Should().Be(naamWerdGewijzigd.Data.Naam);
        doc.DatumLaatsteAanpassing.Should().Be(naamWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(naamWerdGewijzigd.Sequence, naamWerdGewijzigd.Version));}
}
