namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using EventStore;
using Scenarios.EventsInDb;

public class EventsInDbScenariosFixture : AdminApiFixture
{
    public readonly V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields = new();

    public readonly V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields
        V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields = new();

    public readonly V003_FeitelijkeVerenigingWerdGeregistreerd_ForUseWithNoChanges
        V003FeitelijkeVerenigingWerdGeregistreerdForUseWithNoChanges = new();

    public readonly V004_AlleBasisGegevensWerdenGewijzigd V004AlleBasisGegevensWerdenGewijzigd = new();

    public readonly V005_FeitelijkeVerenigingWerdGeregistreerd_ForUseWithETagMatching
        V005FeitelijkeVerenigingWerdGeregistreerdForUseWithETagMatching = new();

    public readonly V006_ContactgegevenWerdToegevoegd V006ContactgegevenWerdToegevoegd = new();

    public readonly V007_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven
        V007FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven = new();

    public readonly V008_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven
        V008FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven = new();

    public readonly V009_FeitelijkeVerenigingWerdGeregistreerd_ForDuplicateForce
        V009FeitelijkeVerenigingWerdGeregistreerdForDuplicateForce = new();

    public readonly V010_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields V010FeitelijkeVerenigingWerdGeregistreerdWithAllFields = new();

    public readonly V011_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForRemovingVertegenwoordiger
        V011FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForRemovingVertegenwoordiger = new();

    public readonly V012_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger
        V012FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger = new();

    public readonly V013_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForDuplicateCheck
        V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck = new();

    public readonly V014_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens
        V014FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens = new();

    public readonly V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens
        V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens = new();

    public readonly V020_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateDetection
        V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection = new();

    public readonly V021_FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroomScenario
        V021FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroom = new();

    public readonly V022_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie
        V022FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatie = new();

    public readonly V023_LocatieWerdToegevoegd V023LocatieWerdToegevoegd = new();

    public readonly V024_FeitelijkeVerenigingWerdGeregistreerd_WithLocatie_ForRemovingLocatie
        V024FeitelijkeVerenigingWerdGeregistreerdWithLocatieForRemovingLocatie = new();

    public readonly V025_LocatieWerdVerwijderd V025LocatieWerdVerwijderd = new();

    public readonly V026_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen
        V026FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigen = new();

    public readonly V027_LocatieWerdGewijzigd V027LocatieWerdGewijzigd = new();

    public readonly V028_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd V028VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
        new();

    public readonly V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data
        V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData = new();

    public readonly V030_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Invalid_Data
        V030VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithInvalidData = new();

    public readonly V031_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForAddingLocatie
        V031VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForAddingLocatie = new();

    public readonly V032_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForWijzigen
        V032VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithLocatiesForWijzigen = new();

    public readonly V033_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForVerwijderen
        V033VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithLocatiesForVerwijderen = new();

    public readonly V034_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForAddingLocatie
        V034VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMaatschappelijkeZetelForAddingLocatie = new();

    public readonly V035_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForAddingContactgegeven
        V035VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForAddingContactgegeven = new();

    public readonly V036_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForWijzigenContactgegeven
        V036VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForWijzigenContactgegeven = new();

    public readonly V037_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForVerwijderContactgegeven
        V037VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForVerwijderContactgegeven = new();

    public readonly V038_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
        V038VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens = new();

    public readonly V040_FeitelijkeVerenigingWerdGeregistreerd_ForStoppen V040FeitelijkeVerenigingWerdGeregistreerdForStoppen = new();
    public readonly V041_FeitelijkeVerenigingWerdGestopt V041FeitelijkeVerenigingWerdGestopt = new();

    public readonly V042_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger
        V042VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger = new();

    public readonly V043_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForWijzigen
        V043VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMaatschappelijkeZetelForWijzigen = new();

    public readonly V044_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO
        V044VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelVolgensKbo = new();

    public readonly V045_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_ContactgegevenFromKbo_For_Wijzigen
        V045VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithContactgegevenFromKboForWijzigen = new();

    public readonly V046_FeitelijkeVerenigingWerdGeregistreerd_ForWijzigStartdatum
        V046FeitelijkeVerenigingWerdGeregistreerdForWijzigStartdatum = new();

    public readonly V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer
        V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer = new();

    public readonly V056_VerenigingWerdGeregistreerd_And_Gestopt_For_DuplicateDetection
        V056VerenigingWerdGeregistreerdAndGestoptForDuplicateDetection = new();

    public readonly V057_VerenigingWerdGeregistreerd_With_KboLocatie_For_DuplicateDetection
        V057VerenigingWerdGeregistreerdWithKboLocatieForDuplicateDetection = new();

    public readonly V058_FeitelijkeVerenigingWerdGeregistreerd_ForRemoval
        V058FeitelijkeVerenigingWerdGeregistreerdForRemoval = new();

    public readonly V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved
        V059FeitelijkeVerenigingWerdGeregistreerdAndRemoved = new();

    public readonly V060_VerenigingWerdGeregistreerd_And_Verwijderd_For_DuplicateDetection
        V060VerenigingWerdGeregistreerdAndVerwijderdForDuplicateDetection = new();

    public readonly V061_VerenigingWerdGeregistreerd_And_Verwijderd_And_FollowedByUpdates
        V061VerenigingWerdGeregistreerdAndVerwijderdAndFollowedByUpdates = new();

    public readonly V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced
        V062VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndSynced = new();

    public readonly V063_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_StartdatumWerdGewijzigd
        V063VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndStartdatumWerdGewijzigd = new();

    public readonly V064_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_EinddatumWerdGewijzigd
        V064VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndEinddatumWerdGewijzigd = new();

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[]
        {
            V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields,
            V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields,
            V003FeitelijkeVerenigingWerdGeregistreerdForUseWithNoChanges,
            V004AlleBasisGegevensWerdenGewijzigd,
            V005FeitelijkeVerenigingWerdGeregistreerdForUseWithETagMatching,
            V006ContactgegevenWerdToegevoegd,
            V007FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven,
            V008FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven,
            V009FeitelijkeVerenigingWerdGeregistreerdForDuplicateForce,
            V010FeitelijkeVerenigingWerdGeregistreerdWithAllFields,
            V011FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForRemovingVertegenwoordiger,
            V012FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger,
            V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck,
            V014FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens,
            V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens,
            V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection,
            V021FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroom,
            V022FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatie,
            V023LocatieWerdToegevoegd,
            V024FeitelijkeVerenigingWerdGeregistreerdWithLocatieForRemovingLocatie,
            V025LocatieWerdVerwijderd,
            V026FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigen,
            V027LocatieWerdGewijzigd,
            V028VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData,
            V030VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithInvalidData,
            V031VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForAddingLocatie,
            V032VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithLocatiesForWijzigen,
            V033VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithLocatiesForVerwijderen,
            V034VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMaatschappelijkeZetelForAddingLocatie,
            V035VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForAddingContactgegeven,
            V036VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForWijzigenContactgegeven,
            V037VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForVerwijderContactgegeven,
            V038VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens,
            V040FeitelijkeVerenigingWerdGeregistreerdForStoppen,
            V041FeitelijkeVerenigingWerdGestopt,
            V042VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger,
            V043VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMaatschappelijkeZetelForWijzigen,
            V044VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelVolgensKbo,
            V045VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithContactgegevenFromKboForWijzigen,
            V046FeitelijkeVerenigingWerdGeregistreerdForWijzigStartdatum,
            V056VerenigingWerdGeregistreerdAndGestoptForDuplicateDetection,
            V057VerenigingWerdGeregistreerdWithKboLocatieForDuplicateDetection,
            V058FeitelijkeVerenigingWerdGeregistreerdForRemoval,
            V059FeitelijkeVerenigingWerdGeregistreerdAndRemoved,
            V060VerenigingWerdGeregistreerdAndVerwijderdForDuplicateDetection,
            V061VerenigingWerdGeregistreerdAndVerwijderdAndFollowedByUpdates,
            V062VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndSynced,
            V063VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndStartdatumWerdGewijzigd,
            V064VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndEinddatumWerdGewijzigd,
        };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }

        foreach (var (vCode, events) in V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer
                    .EventsPerVCode)
        {
            await AddEvents(vCode, events);
        }
    }
}

public class AdminApiScenarioFixture : AdminApiFixture
{
    protected override Task Given()
        => Task.CompletedTask;

    public async Task<StreamActionResult> Apply(IEventsInDbScenario scenario)
        => await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
}
