namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using Events;
using Vereniging;
using AutoFixture;
using Common.AutoFixture;
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
