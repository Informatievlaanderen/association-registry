namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Events;

public class VzerMetActieveErkenningWerdVerlopenScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeactiveerd ErkenningWerdGeactiveerd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }
    public ErkenningWerdVerlopen ErkenningWerdVerlopen { get; }
    public VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend { get; }

    public VzerMetActieveErkenningWerdVerlopenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdGeactiveerd = AutoFixture.Create<ErkenningWerdGeactiveerd>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        VerenigingWerdErkend = new VerenigingWerdErkend();

        ErkenningWerdVerlopen = AutoFixture.Create<ErkenningWerdVerlopen>() with
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
                ErkenningWerdVerlopen,
                VerenigingWerdNietLangerErkend
            ),
        ];
}
