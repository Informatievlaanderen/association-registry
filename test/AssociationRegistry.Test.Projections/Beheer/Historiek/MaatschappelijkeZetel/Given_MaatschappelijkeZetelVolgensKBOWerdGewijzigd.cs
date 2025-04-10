namespace AssociationRegistry.Test.Projections.Beheer.Historiek.MaatschappelijkeZetel;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.MaatschappelijkeZetelVolgensKbo;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    BeheerHistoriekScenarioFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario>
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
                                               Beschrijving: "Maatschappelijke zetel volgens KBO werd gewijzigd.",
                                               nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd),
                                               fixture.Scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
