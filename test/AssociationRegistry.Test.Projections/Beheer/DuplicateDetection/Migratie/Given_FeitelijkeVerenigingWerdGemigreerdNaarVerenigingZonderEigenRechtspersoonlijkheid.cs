namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Migratie;

using Scenario.Migratie;
using Vereniging;


[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(DuplicateDetectionScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : DuplicateDetectionClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Verenigingtype_Is_Vzer()
        => fixture.Result.VerenigingsTypeCode.Should().Be(Verenigingstype.VZER.Code);

    [Fact]
    public void Verenigingsubtype_Is_NietBepaald()
        => fixture.Result.VerenigingssubtypeCode.Should().Be(string.Empty);
}
