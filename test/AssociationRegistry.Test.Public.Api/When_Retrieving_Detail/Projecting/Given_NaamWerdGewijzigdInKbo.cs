namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging;
using Vereniging.Verenigingstype;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_NaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_naam()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var naamWerdGewijzigdInKbo = fixture.Create<TestEvent<NaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(naamWerdGewijzigdInKbo, doc);

        doc.Naam.Should().Be(naamWerdGewijzigdInKbo.Data.Naam);
    }
}

[UnitTest]
public class Given_RechtsvormWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_rechtsvorm()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var rechtsvormWerdGewijzigdInKbo = fixture.Create<TestEvent<RechtsvormWerdGewijzigdInKBO>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(rechtsvormWerdGewijzigdInKbo, doc);

        doc.Verenigingstype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.VerenigingsType
        {
            Code = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        });

        doc.Rechtsvorm.Should().Be(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm);
    }
}

[UnitTest]
public class Given_KorteNaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_naam()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var korteNaamWerdGewijzigdInKbo = fixture.Create<TestEvent<KorteNaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(korteNaamWerdGewijzigdInKbo, doc);

        doc.KorteNaam.Should().Be(korteNaamWerdGewijzigdInKbo.Data.KorteNaam);
    }
}
