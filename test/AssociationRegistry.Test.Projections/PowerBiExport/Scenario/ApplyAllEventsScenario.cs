namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;

public class ApplyAllEventsScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }

    public ApplyAllEventsScenario()
    {
        FeitelijkeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(FeitelijkeVerenigingWerdGeregistreerd.VCode, FeitelijkeVerenigingWerdGeregistreerd),
        new(FeitelijkeVerenigingWerdGeregistreerd.VCode, GetEvents(FeitelijkeVerenigingWerdGeregistreerd.VCode)),
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            GetEvents(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)),
    ];

    private IEvent[] GetEvents(string vCode) =>
    [
        AutoFixture.Create<NaamWerdGewijzigd>() with { VCode = vCode },
        AutoFixture.Create<LocatieWerdToegevoegd>(),
        AutoFixture.Create<LocatieWerdToegevoegd>(),
        AutoFixture.Create<VerenigingWerdGestopt>(),
    ];
}
