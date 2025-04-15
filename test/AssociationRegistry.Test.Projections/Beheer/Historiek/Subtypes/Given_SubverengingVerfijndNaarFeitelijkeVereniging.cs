namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Subtypes;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.Subtypes;
using Events;

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
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Subtype werd verfijnd naar feitelijke vereniging.",
                                               nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
                                               fixture.Scenario.VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
