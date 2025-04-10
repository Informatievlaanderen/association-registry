namespace AssociationRegistry.Test.Projections.Beheer.Historiek.MaatschappelijkeZetel;

using Admin.Schema.Historiek;
using Events;
using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdGewijzigdInKbo(
    BeheerHistoriekScenarioFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario>
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
                                               Beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd gewijzigd in KBO.",
                                               nameof(MaatschappelijkeZetelWerdGewijzigdInKbo),
                                               fixture.Scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
