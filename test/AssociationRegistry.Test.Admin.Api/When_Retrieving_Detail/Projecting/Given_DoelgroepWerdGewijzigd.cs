namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_DoelgroepWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_doelgroep()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var doelgroepWerdGewijzigd = fixture.Create<TestEvent<DoelgroepWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(doelgroepWerdGewijzigd, doc);

        doc.Doelgroep.Should().BeEquivalentTo(doelgroepWerdGewijzigd.Data.Doelgroep);
        doc.DatumLaatsteAanpassing.Should().Be(doelgroepWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(doelgroepWerdGewijzigd.Sequence, doelgroepWerdGewijzigd.Version));}
}
