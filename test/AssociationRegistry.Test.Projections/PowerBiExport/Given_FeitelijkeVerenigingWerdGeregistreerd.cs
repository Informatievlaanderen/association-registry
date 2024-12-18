namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using KellermanSoftware.CompareNetObjects;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGeregistreerd(PowerBiScenarioFixture<FeitelijkeVerenigingWerdGeregistreerdScenario> fixture)
    : PowerBiScenarioClassFixture<FeitelijkeVerenigingWerdGeregistreerdScenario>
{
    [Fact]
    public void ARecordIsStored_With_VCodeAndNaam()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingWerdGeregistreerd.VCode);
        fixture.Result.Naam.Should().Be(fixture.Scenario.VerenigingWerdGeregistreerd.Naam);
    }

    [Fact]
    public void ARecordIsStored_With_Hoofdactiviteiten()
        => fixture.Result
                  .HoofdactiviteitenVerenigingsloket
                  .ShouldCompare(
                       fixture.Scenario
                              .VerenigingWerdGeregistreerd
                              .HoofdactiviteitenVerenigingsloket
                              .Select(x => new HoofdactiviteitVerenigingsloket
                               {
                                   Naam = x.Naam,
                                   Code = x.Code,
                               })
                              .ToArray()
                   );

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();
    }
}
