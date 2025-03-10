namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Verenigingssubtypes;

using AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;
using AssociationRegistry.Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingZonderRechtspersoonlijkheidWerdRegistreerd(DuplicateDetectionScenarioFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario>
{
    [Fact]
    public void Verenigingsubtype_Is_NogNietBepaald()
        => fixture.Result.VerenigingssubtypeCode.Should().Be(Verenigingssubtype.FeitelijkeVereniging.Code);
}
