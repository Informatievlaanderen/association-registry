namespace AssociationRegistry.Test.Projections.Beheer.Detail.RechtsVormWerdGewijzigdInKbo;

using AssociationRegistry.Test.Projections.Scenario.NaamWerdGewijzigd.Kbo;
using Public.Schema.Detail;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_RechtsvormWerdGewijzigdInKBO(
    BeheerDetailScenarioFixture<RechtsvormWerdGewijzigdInKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<RechtsvormWerdGewijzigdInKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Rechtsvorm.Should()
               .BeEquivalentTo(fixture.Scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm);

        fixture.Result.Verenigingstype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.Types.Verenigingstype
        {
            Code = Verenigingstype.Parse(fixture.Scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(fixture.Scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm).Naam,
        });
    }
}
