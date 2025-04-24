namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class LocatieWerdGewijzigdEventsScenario : ScenarioBase
{
    private LocatieWerdToegevoegd? _locatieWerdToegevoegd;
    public LocatieWerdGewijzigd LocatieWerdGewijzigd { get; }
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }

    public LocatieWerdGewijzigdEventsScenario()
    {
        FeitelijkeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        _locatieWerdToegevoegd = AutoFixture.Create<LocatieWerdToegevoegd>();

        LocatieWerdGewijzigd = AutoFixture.Create<LocatieWerdGewijzigd>() with
        {
            Locatie = _locatieWerdToegevoegd.Locatie with
            {
                Naam = "Nieuwe naam",
            },
        };
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(FeitelijkeVerenigingWerdGeregistreerd.VCode, FeitelijkeVerenigingWerdGeregistreerd),
        new(FeitelijkeVerenigingWerdGeregistreerd.VCode, GetEvents(FeitelijkeVerenigingWerdGeregistreerd.VCode)),
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            GetEvents(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)),
    ];

    private IEvent[] GetEvents(string vCode)
    {

        return
        [
            AutoFixture.Create<NaamWerdGewijzigd>() with { VCode = vCode },
            _locatieWerdToegevoegd,
            AutoFixture.Create<LocatieWerdToegevoegd>(),
            LocatieWerdGewijzigd,
        ];
    }
}
