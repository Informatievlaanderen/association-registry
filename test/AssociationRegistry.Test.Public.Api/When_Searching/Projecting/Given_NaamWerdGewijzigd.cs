namespace AssociationRegistry.Test.Public.Api.When_Searching.Projecting;

using Events;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
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
        var projector = new ElasticEventProjection(new VerenigingBrolFeeder());

        var doc = fixture.Create<VerenigingDocument>();

        projector.Apply(naamWerdGewijzigd, doc);

        doc.Naam.Should().Be(naamWerdGewijzigd.Data.Naam);
    }
}
