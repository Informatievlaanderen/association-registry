namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using Common.Framework;
using Newtonsoft.Json;
using Scenarios;

public class GivenEventsFixture : PublicApiFixture
{
    public readonly V001_FeitelijkeVerenigingWerdGeregistreerdScenario V001FeitelijkeVerenigingWerdGeregistreerdScenario = new();

    public readonly V002_FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario
        V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();

    public readonly V003_BasisgegevensWerdenGewijzigdScenario V003BasisgegevensWerdenGewijzigdScenario = new();

    public readonly V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario
        V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario = new();

    public readonly V005_ContactgegevenWerdToegevoegdScenario V005ContactgegevenWerdToegevoegdScenario = new();

    public readonly V006_VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario
        V006VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario = new();

    public readonly V010_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario
        V010FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario = new();

    public readonly V011_LocatieWerdToegevoegdScenario V011LocatieWerdToegevoegdScenario = new();
    public readonly V012_LocatieWerdVerwijderdScenario V012LocatieWerdVerwijderdScenario = new();
    public readonly V013_LocatieWerdGewijzigdScenario V013LocatieWerdGewijzigdScenario = new();

    public readonly V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data_Scenario
        V014VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllDataScenario = new();

    public readonly V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
        V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens = new();

    public readonly V016_VerenigingWerdGestopt V016VerenigingWerdGestopt = new();

    public readonly V017_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel_Scenario
        V017VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelScenario = new();

    public readonly V018_FeitelijkeVerenigingWerdVerwijderdScenario V018_FeitelijkeVerenigingWerdVerwijderdScenario = new();
    public readonly V019_Vereniging19ForSearchScenario V019Vereniging19ForSearchScenario = new();
    public readonly V020_Vereniging20ForSearchScenario V020Vereniging20ForSearchScenario = new();

    public readonly V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd V021VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd =
        new();

    public readonly V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario V022LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario = new();

    public readonly V023_WerdUitgeschrevenUitPubliekeDatastroomScenario_And_NaamWerdGewijzigd
        V023WerdUitgeschrevenUitPubliekeDatastroomScenarioAndNaamWerdGewijzigd = new();

    public readonly V024_FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario
        V024FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario = new();

    private IEnumerable<IScenario> Scenarios
        => new IScenario[]
        {
            V001FeitelijkeVerenigingWerdGeregistreerdScenario,
            V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario,
            V003BasisgegevensWerdenGewijzigdScenario,
            V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario,
            V005ContactgegevenWerdToegevoegdScenario,
            V006VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
            V010FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario,
            V011LocatieWerdToegevoegdScenario,
            V012LocatieWerdVerwijderdScenario,
            V013LocatieWerdGewijzigdScenario,
            V014VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllDataScenario,
            V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens,
            V016VerenigingWerdGestopt,
            V017VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelScenario,
            V018_FeitelijkeVerenigingWerdVerwijderdScenario,
            V019Vereniging19ForSearchScenario,
            V020Vereniging20ForSearchScenario,
            V021VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd,
            V022LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario,
            V023WerdUitgeschrevenUitPubliekeDatastroomScenarioAndNaamWerdGewijzigd,
            V024FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario,
        };

    public override async ValueTask InitializeAsync()
    {
        foreach (var scenario in Scenarios)
        {
            await AddEvents(
                scenario.VCode,
                scenario.GetEvents(),
                scenario.GetCommandMetadata());
        }


        await ProjectionSequenceGuardian.EnsureAllProjectionsAreUpToDate(ProjectionsDocumentStore, MaxSequence, ElasticClient);

        var verengingenCount = 0;
        var counter = 0;

        while (verengingenCount <= 0 && counter < 20)
        {
            counter++;

            var response = await PublicApiClient.Search($"vCode:{Scenarios.Last().VCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var searchVerenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(responseContent);
            verengingenCount = searchVerenigingenResponse.Verenigingen.Length;

            await Task.Delay(counter * 100 + 600);

            if(counter == 20 && verengingenCount <= 0)
                throw new Exception("Not all scenarios projected after 20 attempts. Last response: " + responseContent);
        }
    }
}
