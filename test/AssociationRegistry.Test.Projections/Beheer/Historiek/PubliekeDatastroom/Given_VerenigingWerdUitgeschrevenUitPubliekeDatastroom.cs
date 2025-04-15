namespace AssociationRegistry.Test.Projections.Beheer.Historiek.PubliekeDatastroom;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.PubliekeDatastroom;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitgeschrevenUitPubliekeDatastroom(
    BeheerHistoriekScenarioFixture<VerenigingWerdUitgeschrevenUitPubliekeDatastroomScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingWerdUitgeschrevenUitPubliekeDatastroomScenario>
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
                                               Beschrijving: "Vereniging werd uitgeschreven uit de publieke datastroom.",
                                               nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom),
                                               fixture.Scenario.VerenigingWerdUitgeschrevenUitPubliekeDatastroom,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
