namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VzerMetActieveErkenningenWerdVerwijderdEnActieveBehoudenScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public ErkenningWerdGeregistreerd ActieveErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd TeVerwijderenActieveErkenningWerdVerwijderd { get; }
    public ErkenningWerdVerwijderd ErkenningWerdVerwijderd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }

    public VzerMetActieveErkenningenWerdVerwijderdEnActieveBehoudenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ActieveErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Actief.Value,
        };

        TeVerwijderenActieveErkenningWerdVerwijderd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Actief.Value,
        };

        ErkenningWerdVerwijderd = AutoFixture.Create<ErkenningWerdVerwijderd>() with
        {
            ErkenningId = TeVerwijderenActieveErkenningWerdVerwijderd.ErkenningId,
        };

        VerenigingWerdErkend = new VerenigingWerdErkend();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ActieveErkenningWerdGeregistreerd,
                TeVerwijderenActieveErkenningWerdVerwijderd,
                VerenigingWerdErkend,
                ErkenningWerdVerwijderd
            ),
        ];
}
