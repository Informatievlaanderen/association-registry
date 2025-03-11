namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Registratie;

using Scenario.Registratie;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijnd(DuplicateDetectionScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Verenigingtype_Is_Vzer()
        => fixture.Result.VerenigingsTypeCode.Should().Be(Verenigingstype.VZER.Code);

    [Fact]
    public void Verenigingsubtype_Is_NietBepaald()
        => fixture.Result.VerenigingssubtypeCode.Should().Be(Verenigingssubtype.NietBepaald.Code);
}
