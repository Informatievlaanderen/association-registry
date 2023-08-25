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
public class Given_KorteBeschrijvingWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteBeschrijving()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var korteBeschrijvingWerdGewijzigd = fixture.Create<TestEvent<KorteBeschrijvingWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        doc.KorteBeschrijving.Should().Be(korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving);
        doc.DatumLaatsteAanpassing.Should().Be(korteBeschrijvingWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(korteBeschrijvingWerdGewijzigd.Sequence, korteBeschrijvingWerdGewijzigd.Version));}
}

[UnitTest]
public class Given_RoepnaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteBeschrijving()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var roepnaamWerdGewijzigd = fixture.Create<TestEvent<RoepnaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(roepnaamWerdGewijzigd, doc);

        doc.Roepnaam.Should().Be(roepnaamWerdGewijzigd.Data.Roepnaam);
        doc.DatumLaatsteAanpassing.Should().Be(roepnaamWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(roepnaamWerdGewijzigd.Sequence, roepnaamWerdGewijzigd.Version));}
}
