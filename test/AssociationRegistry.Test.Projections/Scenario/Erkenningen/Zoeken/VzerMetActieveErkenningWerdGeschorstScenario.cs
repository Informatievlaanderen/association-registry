namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using Events;

public class VzerMetActieveErkenningWerdGeschorstScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeactiveerd ErkenningWerdGeactiveerd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }
    public ErkenningWerdGeschorst ErkenningWerdGeschorst { get; }
    public VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend { get; }

    public VzerMetActieveErkenningWerdGeschorstScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdGeactiveerd = AutoFixture.Create<ErkenningWerdGeactiveerd>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        VerenigingWerdErkend = new VerenigingWerdErkend();

        ErkenningWerdGeschorst = AutoFixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
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
                ErkenningWerdGeschorst,
                VerenigingWerdNietLangerErkend
            ),
        ];
}
