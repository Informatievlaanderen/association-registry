namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Scenario.Verwijdering;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdVerwijderd(
    VertegenwoordigersPerVCodeScenarioFixture<VerenigingWerdVerwijderdScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<VerenigingWerdVerwijderdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Not_Removed()
        => fixture.Result
                  .Should()
                  .NotBeNull();

    [Fact]
    public void Vereniging_Is_Verwijderd()
        => fixture.Result.VerenigingIsVerwijderd
                  .Should()
                  .BeTrue();
}


