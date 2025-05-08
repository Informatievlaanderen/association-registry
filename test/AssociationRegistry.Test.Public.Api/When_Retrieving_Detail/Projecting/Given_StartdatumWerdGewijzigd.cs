namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_StartdatumWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_startdatum()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Startdatum.Should().Be(startdatumWerdGewijzigd.Data.Startdatum);
    }
}

public class Given_StartdatumWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_startdatum()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigdInKbo>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Startdatum.Should().Be(startdatumWerdGewijzigd.Data.Startdatum);
    }
}
