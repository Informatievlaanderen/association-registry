namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;
using Formatters = AssociationRegistry.Admin.Api.Infrastructure.Extensions.Formatters;

[UnitTest]
public class Given_KorteNaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteNaam()
    {
        var fixture = new Fixture().CustomizeAll();
        var korteNaamWerdGewijzigd = fixture.Create<TestEvent<KorteNaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(korteNaamWerdGewijzigd, doc);

        doc.KorteNaam.Should().Be(korteNaamWerdGewijzigd.Data.KorteNaam);
        doc.DatumLaatsteAanpassing.Should().Be(Formatters.ToBelgianDate(korteNaamWerdGewijzigd.Tijdstip));
        doc.Metadata.Should().BeEquivalentTo(new Metadata(korteNaamWerdGewijzigd.Sequence, korteNaamWerdGewijzigd.Version));}
}
