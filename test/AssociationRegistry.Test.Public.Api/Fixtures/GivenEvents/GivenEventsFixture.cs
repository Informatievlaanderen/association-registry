namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

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
    public readonly V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd V021VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd = new();
    public readonly V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario V022LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario = new();
    public readonly V023_WerdUitgeschrevenUitPubliekeDatastroomScenario_And_NaamWerdGewijzigd V023WerdUitgeschrevenUitPubliekeDatastroomScenarioAndNaamWerdGewijzigd = new();

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
            V023WerdUitgeschrevenUitPubliekeDatastroomScenarioAndNaamWerdGewijzigd
        };

    public override async Task InitializeAsync()
    {
        foreach (var scenario in Scenarios)
        {
            await AddEvents(
                scenario.VCode,
                scenario.GetEvents(),
                scenario.GetCommandMetadata());
        }
    }
}
