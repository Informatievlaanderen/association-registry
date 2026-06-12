namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Subtypes;

using Admin.Schema.Historiek;
using Events;
using Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingssubtypeWerdTerugGezetNaarNietBepaald(
    BeheerHistoriekScenarioFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<SubverenigingWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Historiek_Saved_BeheerVerenigingHistoriekGebeurtenis()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Subtype werd teruggezet naar niet bepaald.",
                                               nameof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
                                               fixture.Scenario.VerenigingssubtypeWerdTerugGezetNaarNietBepaald,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
