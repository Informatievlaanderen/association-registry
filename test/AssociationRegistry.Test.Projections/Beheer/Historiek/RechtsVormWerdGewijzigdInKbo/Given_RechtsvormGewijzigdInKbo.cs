namespace AssociationRegistry.Test.Projections.Beheer.Historiek.RechtsVormWerdGewijzigdInKbo;

using Admin.Schema.Historiek;
using DecentraalBeheer.Vereniging;
using Events;
using Scenario.NaamWerdGewijzigd.Kbo;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_RechtsvormWerdGewijzigdInKBO(
    BeheerHistoriekScenarioFixture<RechtsvormWerdGewijzigdInKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<RechtsvormWerdGewijzigdInKBOScenario>
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
                                               Beschrijving: $"In KBO werd de rechtsvorm gewijzigd naar '{Verenigingstype.Parse(fixture.Scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm).Naam}'.",
                                               nameof(RechtsvormWerdGewijzigdInKBO),
                                               fixture.Scenario.RechtsvormWerdGewijzigdInKBO,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
