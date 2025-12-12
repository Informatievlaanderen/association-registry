namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderd(
    VertegenwoordigersPerVCodeScenarioFixture<VertegenwoordigerWerdVerwijderdScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<VertegenwoordigerWerdVerwijderdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result!.VertegenwoordigersData
                  .FirstOrDefault(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdVerwijderd.VertegenwoordigerId)
                  .Should()
                  .BeNull();
}
