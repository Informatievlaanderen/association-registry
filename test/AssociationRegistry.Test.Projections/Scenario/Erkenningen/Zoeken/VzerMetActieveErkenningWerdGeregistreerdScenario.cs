namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Events;

public class VzerMetActieveErkenningWerdGeregistreerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }

    public VzerMetActieveErkenningWerdGeregistreerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
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
                VerenigingWerdErkend
            ),
        ];
}
