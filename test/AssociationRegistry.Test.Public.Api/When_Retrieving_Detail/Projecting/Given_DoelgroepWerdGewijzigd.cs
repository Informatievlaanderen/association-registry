namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_DoelgroepWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_doelgroep()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var doelgroepWerdGewijzigd = fixture.Create<TestEvent<DoelgroepWerdGewijzigd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(doelgroepWerdGewijzigd, doc);

        doc.Doelgroep.Should().BeEquivalentTo(doelgroepWerdGewijzigd.Data.Doelgroep);
    }
}
