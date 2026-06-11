namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingMetActieveEnVerlopenErkenningActieveErkenningWerdVerwijderdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public ErkenningWerdGeregistreerd VerlopenErkenningWerdGerigstreerd { get; }
    public ErkenningWerdGeregistreerd TeVerwijderenActieveErkenningWerdVerwijderd { get; }
    public ErkenningWerdVerwijderd ErkenningWerdVerwijderd { get; }

    public VerenigingMetActieveEnVerlopenErkenningActieveErkenningWerdVerwijderdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerlopenErkenningWerdGerigstreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Verlopen.Value,
        };

        TeVerwijderenActieveErkenningWerdVerwijderd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Actief.Value,
        };

        ErkenningWerdVerwijderd = AutoFixture.Create<ErkenningWerdVerwijderd>() with
        {
            ErkenningId = TeVerwijderenActieveErkenningWerdVerwijderd.ErkenningId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                TeVerwijderenActieveErkenningWerdVerwijderd,
                VerlopenErkenningWerdGerigstreerd,
                ErkenningWerdVerwijderd
            ),
        ];
}
