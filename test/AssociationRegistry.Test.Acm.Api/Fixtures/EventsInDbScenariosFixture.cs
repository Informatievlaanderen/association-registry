namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using AssociationRegistry.Acm.Api;
using Common.Framework;
using JasperFx.Events.Daemon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scenarios;

public class EventsInDbScenariosFixture : AcmApiFixture
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario
        FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario = new();

    public readonly VertegenwoordigerWerdToegevoegd_EventsInDbScenario VertegenwoordigerWerdToegevoegdEventsInDbScenario = new();

    public readonly NaamWerdGewijzigd_And_VertegenwoordigerWerdToegevoegd_EventsInDbScenario
        NaamWerdGewijzigdAndVertegenwoordigerWerdToegevoegdEventsInDbScenario = new();

    public readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario AlleBasisGegevensWerdenGewijzigdEventsInDbScenario = new();
    public readonly VertegenwoordigerWerdVerwijderd_EventsInDbScenario VertegenwoordigerWerdVerwijderdEventsInDbScenario = new();

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_EventsInDbScenario
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEventsInDbScenario = new();

    public readonly FeitelijkeVerenigingWerdGestopt_EventsInDbScenario
        FeitelijkeVerenigingWerdGestoptEventsInDbScenario = new();

    public readonly FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario
        FeitelijkeVerenigingWerdVerwijderdEventsInDbScenario = new();

    public readonly VerenigingMetRechtspersoonlijkheid_WithAllFields_EventsInDbScenario
        VerenigingMetRechtspersoonlijkheidWithAllFieldsEventsInDbScenario = new();

    public readonly RechtsvormWerdGewijzigdInKBO_EventsInDbScenario
        RechtsvormWerdGewijzigdInKBOEventsInDbScenario = new();

    public long MaxSequence { get; private set; } = 0;

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[]
        {
            FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario,
            VertegenwoordigerWerdToegevoegdEventsInDbScenario,
            AlleBasisGegevensWerdenGewijzigdEventsInDbScenario,
            NaamWerdGewijzigdAndVertegenwoordigerWerdToegevoegdEventsInDbScenario,
            VertegenwoordigerWerdVerwijderdEventsInDbScenario,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEventsInDbScenario,
            FeitelijkeVerenigingWerdGestoptEventsInDbScenario,
            FeitelijkeVerenigingWerdVerwijderdEventsInDbScenario,
            VerenigingMetRechtspersoonlijkheidWithAllFieldsEventsInDbScenario,
            RechtsvormWerdGewijzigdInKBOEventsInDbScenario,
        };

        using var daemon = await StartDaemonBeforeAddingEvents();

        MaxSequence = 0;

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
            MaxSequence = Math.Max(MaxSequence, scenario.Result.Sequence.Value);
        }
        var logger = ServiceProvider.GetRequiredService<ILogger<Program>>();

        await ProjectionSequenceGuardian.EnsureAllProjectionsAreUpToDate(DocumentStore, "acm", MaxSequence, logger);
    }
    private async Task<IProjectionDaemon?> StartDaemonBeforeAddingEvents()
    {
        IProjectionDaemon? daemon = null;

        try
        {
            daemon = await DocumentStore.BuildProjectionDaemonAsync();
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
}
