namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Migratie;

using AssociationRegistry.Admin.Schema.Historiek;
using Events;
using Scenario.Migratie;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(BeheerHistoriekScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
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
                                               Beschrijving: "Feitelijke vereniging werd gemigreerd naar vereniging zonder eigen rechtspersoonlijkheid.",
                                               nameof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid),
                                               fixture.Scenario.FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
