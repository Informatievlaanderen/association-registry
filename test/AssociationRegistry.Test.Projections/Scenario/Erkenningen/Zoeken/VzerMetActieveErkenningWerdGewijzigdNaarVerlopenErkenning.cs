namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VzerMetActieveErkenningWerdGewijzigdNaarVerlopenErkenning : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public ErkenningWerdGeregistreerd VerlopenErkenningWerdGerigstreerd { get; }
    public ErkenningWerdGeregistreerd ActieveErkenningWerdGeregistreerd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }
    public ErkenningWerdGewijzigd ActieveErkenningWerdGewijzigdNaarVerlopenErkenning { get; }
    public VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend { get; }

    public VzerMetActieveErkenningWerdGewijzigdNaarVerlopenErkenning()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerlopenErkenningWerdGerigstreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Verlopen.Value,
        };

        ActieveErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Actief.Value,
        };

        VerenigingWerdErkend = new VerenigingWerdErkend();

        ActieveErkenningWerdGewijzigdNaarVerlopenErkenning = AutoFixture.Create<ErkenningWerdGewijzigd>() with
        {
            ErkenningId = ActieveErkenningWerdGeregistreerd.ErkenningId,
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
                VerlopenErkenningWerdGerigstreerd,
                ActieveErkenningWerdGeregistreerd,
                VerenigingWerdErkend,
                ActieveErkenningWerdGewijzigdNaarVerlopenErkenning,
                VerenigingWerdNietLangerErkend
            ),
        ];
}
