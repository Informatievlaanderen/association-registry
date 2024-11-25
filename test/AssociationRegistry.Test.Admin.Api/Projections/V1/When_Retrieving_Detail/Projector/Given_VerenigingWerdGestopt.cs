namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
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
    }

    [Fact]
    public void Then_It_Changes_The_Einddatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestopt>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdGestopt, doc);

        doc.Einddatum.Should().Be(verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly));
    }
}

[UnitTest]
public class Given_VerenigingWerdGestoptInKBO
{
    [Fact]
    public void Then_It_Changes_The_Status_To_Gestopt()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestoptInKBO>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdGestopt, doc);

        doc.Status.Should().Be(VerenigingStatus.Gestopt);
    }

    [Fact]
    public void Then_It_Changes_The_Einddatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestoptInKBO>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdGestopt, doc);

        doc.Einddatum.Should().Be(verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly));
    }
}
