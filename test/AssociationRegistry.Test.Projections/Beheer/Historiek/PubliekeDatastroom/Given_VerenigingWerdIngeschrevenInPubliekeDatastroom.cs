namespace AssociationRegistry.Test.Projections.Beheer.Historiek.PubliekeDatastroom;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.PubliekeDatastroom;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdIngeschrevenInPubliekeDatastroom(
    BeheerHistoriekScenarioFixture<VerenigingWerdIngeschrevenInPubliekeDatastroomScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingWerdIngeschrevenInPubliekeDatastroomScenario>
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
                                               Beschrijving: "Vereniging werd ingeschreven in de publieke datastroom.",
                                               nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom),
                                               fixture.Scenario.VerenigingWerdIngeschrevenInPubliekeDatastroom,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
