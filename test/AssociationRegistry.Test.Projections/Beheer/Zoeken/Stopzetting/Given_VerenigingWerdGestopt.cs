namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Stopzetting;

using Formats;
using Admin.Schema.Constants;
using Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestopt(BeheerZoekenScenarioFixture<FeitelijkeVerenigingWerdGestoptScenario> fixture)
    : BeheerZoekenScenarioClassFixture<FeitelijkeVerenigingWerdGestoptScenario>
{

    [Fact]
    public void EindDatum_Is_Set()
        => fixture.Result
                  .Einddatum
                  .Should()
                  .BeEquivalentTo(fixture.Scenario.VerenigingWerdGestopt.Einddatum.FormatAsBelgianDate());

    [Fact]
    public void Status_Is_Gestopt()
        => fixture.Result
                  .Status
                  .Should()
                  .BeEquivalentTo(VerenigingStatus.Gestopt);
}
