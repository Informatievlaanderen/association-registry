namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using EventStore;
using Scenarios.EventsInDb;

public class EventsInDbScenariosFixture : AdminApiFixture
{
    public readonly V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields = new();
    public readonly V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields = new();
    public readonly V003_FeitelijkeVerenigingWerdGeregistreerd_ForUseWithNoChanges V003FeitelijkeVerenigingWerdGeregistreerdForUseWithNoChanges = new();
    public readonly V004_AlleBasisGegevensWerdenGewijzigd V004AlleBasisGegevensWerdenGewijzigd = new();
    public readonly V005_FeitelijkeVerenigingWerdGeregistreerd_ForUseWithETagMatching V005FeitelijkeVerenigingWerdGeregistreerdForUseWithETagMatching = new();
    public readonly V006_ContactgegevenWerdToegevoegd V006ContactgegevenWerdToegevoegd = new();
    public readonly V007_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven V007FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven = new();
    public readonly V008_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven V008FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven = new();
    public readonly V009_FeitelijkeVerenigingWerdGeregistreerd_ForDuplicateForce V009FeitelijkeVerenigingWerdGeregistreerdForDuplicateForce = new();
    public readonly V010_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields V010FeitelijkeVerenigingWerdGeregistreerdWithAllFields = new();
    public readonly V011_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForRemovingVertegenwoordiger V011FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForRemovingVertegenwoordiger = new();
    public readonly V012_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger V012FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger = new();
    public readonly V013_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForDuplicateCheck V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck = new();
    public readonly V014_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens V014FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens = new();
    public readonly V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens = new();
    public readonly V016_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_AlsMoeder_VoorRegistratieAfdeling V016VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAlsMoederVoorRegistratieAfdeling = new();
    public readonly V017_AfdelingWerdGeregistreerd_WithMinimalFields_And_Registered_Moeder V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder = new();
    public readonly V019_AfdelingWerdGeregistreerd_WithMinimalFields V019AfdelingWerdGeregistreerdWithMinimalFields = new();
    public readonly V020_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateDetection V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection = new();
    public readonly V021_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario V021FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroom = new();

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
            V016VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAlsMoederVoorRegistratieAfdeling,
            V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder,
            V019AfdelingWerdGeregistreerdWithMinimalFields,
            V020VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForDuplicateDetection,
            V021FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroom,
        };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
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
