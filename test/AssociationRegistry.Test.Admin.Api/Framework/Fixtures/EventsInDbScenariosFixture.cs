namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

using AssociationRegistry.Admin.ProjectionHost;
using Common.Framework;
using EventStore;
using Common.Scenarios.EventsInDb;
using Events;
using JasperFx.Core;
using JasperFx.Events.Daemon;
using Marten.Events.Aggregation;
using Marten.Events.Daemon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

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

    public readonly V065_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_AdresNietUniekInAdressenregister
        V065FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresNietUniekInAdressenregister = new();

    public readonly
        V066_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_AdresWerdOvergenomenUitAdressenregister
        V066FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresWerdOvergenomenUitAdressenregister = new();

    public readonly
        V067_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_AdresWerdNietGevondenInAdressenregister
        V067FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresWerdNietGevondenInAdressenregister = new();

    public readonly V068_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresNietUniekInAdressenregister
        V068FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresNietUniekInAdressenregister = new();

    public readonly V069_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresWerdOvergenomenUitAdressenregister
        V069FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresWerdOvergenomenUitAdressenregister = new();

    public readonly V070_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresWerdNietGevondenInAdressenregister
        V070FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresWerdNietGevondenInAdressenregister = new();

    public readonly V071_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie_For_PostalInformation
        V071FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForPostalInformation = new();

    public readonly V072_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresKonNietOvergenomenWordenUitAdressenregister
        V072FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresKonNietOvergenomenWordenUitAdressenregister = new();

    public readonly V073_AdresWerdOvergenomenUitAdressenregister
        V073AdresWerdOvergenomenUitAdressenregister = new();

    public readonly V074_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd
        V074AdresWerdOvergenomenUitAdressenregisterAndVerenigingWerdVerwijderd = new();

    public readonly V075_AdresWerdGewijzigdInAdressenregister V075AdresWerdGewijzigdInAdressenregister = new();
    public readonly V076_AdresWerdGewijzigdInAdressenregister V076AdresWerdGewijzigdInAdressenregister = new();
    public readonly V077_LocatieDuplicaatWerdVerwijderdNaAdresMatch V077LocatieDuplicaatWerdVerwijderdNaAdresMatch = new();
    public readonly V078_AdresWerdOntkoppeldVanAdressenregister V078AdresWerdOntkoppeldVanAdressenregister = new();

    public readonly V079_FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroom_And_NaamGewijzigdScenario
        V079FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroomAndNaamGewijzigd = new();

    public readonly V080_V081_VerenigingWerdGeregistreerd_And_Gemarkeerd_Als_Dubbel_For_DuplicateDetection
        V080V081VerenigingWerdGeregistreerdAndGemarkeerdAlsDubbelForDuplicateDetection = new();

    public readonly V082_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateForce
        V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce = new();

    public readonly V083_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForDuplicateCheck
        V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck = new();


    public EventsInDbScenariosFixture()
    {
    }

    public long MaxSequence { get; private set; } = 0;



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
            V065FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresNietUniekInAdressenregister,
            V066FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresWerdOvergenomenUitAdressenregister,
            V067FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForAdresWerdNietGevondenInAdressenregister,
            V068FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresNietUniekInAdressenregister,
            V069FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresWerdOvergenomenUitAdressenregister,
            V070FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresWerdNietGevondenInAdressenregister,
            V071FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatieForPostalInformation,
            V072FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresKonNietOvergenomenWordenUitAdressenregister,
            V079FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroomAndNaamGewijzigd,
            V080V081VerenigingWerdGeregistreerdAndGemarkeerdAlsDubbelForDuplicateDetection,
            V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce,
            V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck,
        };

        using var daemon = await PreAddEvents();

        MaxSequence = 0;
        var logger = ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("====== Adding events in database =======");
        foreach (var scenario in scenarios)
        {
            logger.LogInformation("====== Adding scenario {ScenarioName} with VCode {VCode} =======",
                scenario.GetType().Name, scenario.VCode);

            var originalEvents = scenario.GetEvents();
            var eventsWithMigration = originalEvents
                                     .Where(x => x is FeitelijkeVerenigingWerdGeregistreerd).Cast<FeitelijkeVerenigingWerdGeregistreerd>()
                                     .Select(eventToAdd => new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(@eventToAdd.VCode)).ToList();

            scenario.Result = await SaveEvents(scenario.VCode, originalEvents.Concat(eventsWithMigration).ToArray(), scenario.GetCommandMetadata());
            if(scenario.Result.Sequence.HasValue)
            {
                MaxSequence = Math.Max(MaxSequence, scenario.Result.Sequence.Value);
                logger.LogInformation("====== Sequence returned for scenario {ScenarioName} with VCode {VCode}: {Sequence} =======",
                    scenario.GetType().Name, scenario.VCode, MaxSequence);
            }
            else
            {
                logger.LogInformation("====== No sequence returned for scenario {ScenarioName} with VCode {VCode} =======",
                    scenario.GetType().Name, scenario.VCode);
            }
        }


        foreach (var (vCode, events) in V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer
                    .EventsPerVCode)
        {
            var result = await SaveEvents(vCode, events.Append(new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(vCode)).ToArray());
            if(result.Sequence.HasValue)
                MaxSequence = Math.Max(MaxSequence, result.Sequence.Value);
        }

        await ProjectionSequenceGuardian.EnsureAllProjectionsAreUpToDate(ProjectionsDocumentStore, MaxSequence, ElasticClient, logger);
    }

    private async Task<IProjectionDaemon?> PreAddEvents()
    {
        IProjectionDaemon? daemon = null;

        try
        {
            daemon = await ProjectionsDocumentStore.BuildProjectionDaemonAsync();
            await daemon.StartAllAsync();

            if (daemon is null)
                throw new NullReferenceException("Projection daemon cannot be null when adding an event");

            return daemon;
        }
        catch
        {
            daemon?.Dispose();

            throw;
        }
    }

    public async Task Initialize(IEventsInDbScenario scenario)
    {
        using var daemon = await PreAddEvents();

        scenario.Result = await SaveEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());

        if(scenario.Result.Sequence.HasValue)
            MaxSequence = Math.Max(MaxSequence, scenario.Result.Sequence.Value);

        var logger = ServiceProvider.GetRequiredService<ILogger<Program>>();

        await ProjectionSequenceGuardian.EnsureAllProjectionsAreUpToDate(ProjectionsDocumentStore, MaxSequence, ElasticClient, logger);
    }
}

public class AdminApiScenarioFixture : AdminApiFixture
{
    protected override Task Given()
        => Task.CompletedTask;

    public async Task<StreamActionResult> Apply(IEventsInDbScenario scenario)
        => await SaveEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
}
