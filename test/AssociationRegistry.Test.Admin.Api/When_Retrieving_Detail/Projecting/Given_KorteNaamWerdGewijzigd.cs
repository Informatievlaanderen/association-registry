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
public class Given_KorteNaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteNaam()
    {
        var fixture = new Fixture().CustomizeAll();
        var korteNaamWerdGewijzigd = fixture.Create<TestEvent<KorteNaamWerdGewijzigd>>();
        var projector = new BeheerVerenigingDetailProjection();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        projector.Apply(korteNaamWerdGewijzigd, doc);

        doc.KorteNaam.Should().Be(korteNaamWerdGewijzigd.Data.KorteNaam);
        doc.DatumLaatsteAanpassing.Should().Be(korteNaamWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(korteNaamWerdGewijzigd.Sequence, korteNaamWerdGewijzigd.Version));}
}
