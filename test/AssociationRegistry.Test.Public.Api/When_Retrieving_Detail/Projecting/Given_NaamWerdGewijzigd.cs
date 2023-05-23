namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using Events;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using Framework;
using AutoFixture;
using FluentAssertions;
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
        var projector = new PubliekVerenigingDetailProjection();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        projector.Apply(naamWerdGewijzigd, doc);

        doc.Naam.Should().Be(naamWerdGewijzigd.Data.Naam);
        doc.DatumLaatsteAanpassing.Should().Be(naamWerdGewijzigd.Tijdstip.ToBelgianDate());
    }
}
