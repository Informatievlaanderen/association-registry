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
using WellknownFormats = AssociationRegistry.Admin.Api.Constants.WellknownFormats;

[UnitTest]
public class Given_StartdatumWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_startdatum()
    {
        var fixture = new Fixture().CustomizeAll();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Startdatum.Should().Be(startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly));
        doc.DatumLaatsteAanpassing.Should().Be(Formatters.ToBelgianDate(startdatumWerdGewijzigd.Tijdstip));
        doc.Metadata.Should().BeEquivalentTo(new Metadata(startdatumWerdGewijzigd.Sequence, startdatumWerdGewijzigd.Version));}
}
