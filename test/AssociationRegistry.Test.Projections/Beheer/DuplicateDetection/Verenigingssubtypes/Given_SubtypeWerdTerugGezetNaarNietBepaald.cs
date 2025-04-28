namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Verenigingssubtypes;

using AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;
using AssociationRegistry.Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_SubtypeWerdTerugGezetNaarNietBepaald(DuplicateDetectionScenarioFixture<SubtypeWerdTerugGezetNaarNietBepaaldScenario> fixture)
    : DuplicateDetectionClassFixture<SubtypeWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void Verenigingsubtype_Is_NietBepaald()
        => fixture.Result.VerenigingssubtypeCode.Should().Be(VerenigingssubtypeCode.NietBepaald.Code);
}
