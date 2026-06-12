namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Subtypes;

using Admin.Schema.Historiek;
using Events;
using Scenario.Subtypes;

[Collection(nameof(ProjectionContext))]
public class Given_SubverengingVerfijndNaarFeitelijkeVereniging(
    BeheerHistoriekScenarioFixture<SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario>
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
                                               Beschrijving: "Subtype werd verfijnd naar feitelijke vereniging.",
                                               nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
                                               fixture.Scenario.VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
