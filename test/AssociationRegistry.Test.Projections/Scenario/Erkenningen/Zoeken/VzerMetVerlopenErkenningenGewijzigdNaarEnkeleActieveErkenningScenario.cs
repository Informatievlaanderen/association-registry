namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VzerMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public ErkenningWerdGeregistreerd VerlopenErkenningWerdGerigstreerd { get; }
    public ErkenningWerdGeregistreerd TeActiverenVerlopenErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeactiveerd TeActiverenVerlopenErkenningErkenningWerdGeactiveerd { get; }

    public VzerMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerlopenErkenningWerdGerigstreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Verlopen.Value,
        };

        TeActiverenVerlopenErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Verlopen.Value,
        };

        TeActiverenVerlopenErkenningErkenningWerdGeactiveerd = AutoFixture.Create<ErkenningWerdGeactiveerd>() with
        {
            ErkenningId = TeActiverenVerlopenErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerlopenErkenningWerdGerigstreerd,
                TeActiverenVerlopenErkenningWerdGeregistreerd,
                TeActiverenVerlopenErkenningErkenningWerdGeactiveerd
            ),
        ];
}
