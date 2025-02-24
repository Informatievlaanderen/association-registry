namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_StartdatumWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_startdatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Startdatum.Should().Be(startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly));
    }
}

[UnitTest]
public class Given_StartdatumWerdGewijzigdInKBO
{
    [Fact]
    public void Then_it_modifies_the_startdatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Startdatum.Should().Be(startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly));
    }
}
