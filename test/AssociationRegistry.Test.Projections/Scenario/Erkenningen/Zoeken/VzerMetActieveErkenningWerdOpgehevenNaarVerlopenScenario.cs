namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VzerMetActieveErkenningWerdOpgehevenNaarVerlopenScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeactiveerd ErkenningWerdGeactiveerd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }
    public SchorsingVanErkenningWerdOpgeheven SchorsingVanErkenningWerdOpgeheven { get; }
    public VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend { get; }

    public VzerMetActieveErkenningWerdOpgehevenNaarVerlopenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdGeactiveerd = AutoFixture.Create<ErkenningWerdGeactiveerd>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        VerenigingWerdErkend = new VerenigingWerdErkend();

        SchorsingVanErkenningWerdOpgeheven = AutoFixture.Create<SchorsingVanErkenningWerdOpgeheven>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
            Status = ErkenningStatus.Verlopen.Value,
        };

        VerenigingWerdNietLangerErkend = new VerenigingWerdNietLangerErkend();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerd,
                ErkenningWerdGeactiveerd,
                VerenigingWerdErkend,
                SchorsingVanErkenningWerdOpgeheven,
                VerenigingWerdNietLangerErkend
            ),
        ];
}
