namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VzerMetGeschorsteErkenningWerdOpgehevenNaarActiefScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeschorst ErkenningWerdGeschorst { get; }
    public SchorsingVanErkenningWerdOpgeheven SchorsingVanErkenningWerdOpgeheven { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }

    public VzerMetGeschorsteErkenningWerdOpgehevenNaarActiefScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdGeschorst = AutoFixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        SchorsingVanErkenningWerdOpgeheven = AutoFixture.Create<SchorsingVanErkenningWerdOpgeheven>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
            Status = ErkenningStatus.Actief.Value,
        };

        VerenigingWerdErkend = new VerenigingWerdErkend();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerd,
                ErkenningWerdGeschorst,
                SchorsingVanErkenningWerdOpgeheven,
                VerenigingWerdErkend
            ),
        ];
}
