namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_NaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_naam()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var naamWerdGewijzigdInKbo = fixture.Create<TestEvent<NaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(naamWerdGewijzigdInKbo, doc);

        doc.Naam.Should().Be(naamWerdGewijzigdInKbo.Data.Naam);
    }
}
