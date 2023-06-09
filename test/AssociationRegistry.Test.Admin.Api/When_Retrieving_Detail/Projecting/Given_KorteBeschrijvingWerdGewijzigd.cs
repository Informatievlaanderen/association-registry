namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
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
public class Given_KorteBeschrijvingWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteBeschrijving()
    {
        var fixture = new Fixture().CustomizeAll();
        var korteBeschrijvingWerdGewijzigd = fixture.Create<TestEvent<KorteBeschrijvingWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        doc.KorteBeschrijving.Should().Be(korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving);
        doc.DatumLaatsteAanpassing.Should().Be(Formatters.ToBelgianDate(korteBeschrijvingWerdGewijzigd.Tijdstip));
        doc.Metadata.Should().BeEquivalentTo(new Metadata(korteBeschrijvingWerdGewijzigd.Sequence, korteBeschrijvingWerdGewijzigd.Version));}
}
