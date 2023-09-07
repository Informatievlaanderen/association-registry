namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingWerdGestopt
{
    [Fact]
    public void Then_It_Changes_The_Status_To_Gestopt()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestopt>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdGestopt, doc);

        doc.Status.Should().Be(VerenigingStatus.Gestopt);
        doc.DatumLaatsteAanpassing.Should().Be(verenigingWerdGestopt.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(verenigingWerdGestopt.Sequence, verenigingWerdGestopt.Version));
    }

    [Fact]
    public void Then_It_Changes_The_Einddatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestopt>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdGestopt, doc);

        doc.Einddatum.Should().Be(verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly));
        doc.DatumLaatsteAanpassing.Should().Be(verenigingWerdGestopt.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(verenigingWerdGestopt.Sequence, verenigingWerdGestopt.Version));
    }
}
