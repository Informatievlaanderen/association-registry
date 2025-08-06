namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Verenigingssubtypes;

using AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;
using DecentraalBeheer.Vereniging;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(DuplicateDetectionScenarioFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario>
{
    [Fact]
    public void Verenigingsubtype_Is_NietBepaald()
        => fixture.Result.VerenigingssubtypeCode.Should().Be(VerenigingssubtypeCode.FeitelijkeVereniging.Code);
}
