namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Verenigingssubtypes;

using AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;
using AssociationRegistry.Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdVerfijndNaarSubvereniging(DuplicateDetectionScenarioFixture<VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario>
{
    [Fact]
    public void Verenigingsubtype_Is_Subvereniging()
        => fixture.Result.VerenigingssubtypeCode.Should().Be(Verenigingssubtype.Subvereniging.Code);
}
