namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_RechtsvormWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_rechtsvorm()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var rechtsvormWerdGewijzigdInKbo = new TestEvent<RechtsvormWerdGewijzigdInKBO>(fixture.Create<RechtsvormWerdGewijzigdInKBO>());

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(rechtsvormWerdGewijzigdInKbo, doc);

        doc.Rechtsvorm.Should().Be(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm);

        doc.Verenigingstype.Should().BeEquivalentTo(new VerenigingsType
        {
            Code = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        });
    }
}
