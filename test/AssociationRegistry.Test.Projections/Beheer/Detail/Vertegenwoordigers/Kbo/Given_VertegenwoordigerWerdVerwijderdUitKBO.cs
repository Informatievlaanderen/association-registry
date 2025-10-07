namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Kbo;

using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderdUitKBO(
    BeheerDetailScenarioFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        var vertegenwoordiger = fixture.Result.Vertegenwoordigers.SingleOrDefault(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdVerwijderdUitKBO.VertegenwoordigerId);
        vertegenwoordiger.Should().BeNull();
    }
}
