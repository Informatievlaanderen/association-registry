namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Migratie;

using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(DuplicateDetectionScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : DuplicateDetectionClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.VerenigingsTypeCode.Should().Be(Verenigingstype.VZER.Code);
}
