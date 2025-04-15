namespace AssociationRegistry.Test.Projections.Beheer.Historiek.MaatschappelijkeZetel;

using Admin.Schema.Historiek;
using Events;
using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo(
    BeheerHistoriekScenarioFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd overgenomen uit KBO.",
                                               nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo),
                                               fixture.Scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
