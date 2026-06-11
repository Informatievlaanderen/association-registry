namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public ErkenningWerdGeregistreerd VerlopenErkenningWerdGerigstreerd { get; }
    public ErkenningWerdGeregistreerd TeActiverenVerlopenErkenningErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeactiveerd TeActiverenVerlopenErkenningErkenningWerdGeactiveerd { get; }

    public VerenigingMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerlopenErkenningWerdGerigstreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Verlopen.Value,
        };

        TeActiverenVerlopenErkenningErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Verlopen.Value,
        };

        TeActiverenVerlopenErkenningErkenningWerdGeactiveerd = AutoFixture.Create<ErkenningWerdGeactiveerd>() with
        {
            ErkenningId = TeActiverenVerlopenErkenningErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerlopenErkenningWerdGerigstreerd,
                TeActiverenVerlopenErkenningErkenningWerdGeregistreerd,
                TeActiverenVerlopenErkenningErkenningWerdGeactiveerd
            ),
        ];
}
