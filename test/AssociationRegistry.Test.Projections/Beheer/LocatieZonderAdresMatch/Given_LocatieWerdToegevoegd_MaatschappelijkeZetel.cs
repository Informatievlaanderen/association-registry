namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AutoFixture;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdToegevoegd_MaatschappelijkeZetelVolgensKbo(
    LocatiesZonderAdresMatchScenarioFixture<LocatieWerdToegevoegdMaatschappelijkeZetelScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<LocatieWerdToegevoegdMaatschappelijkeZetelScenario>
{
    [Fact]
    public void Then_Locatie_Should_Contain_LocationId()
    {
        var expectedLocatieIds =
            fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.Select(x =>
                x.LocatieId
            );

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
