namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class ErkenningOpvolgersWerdenToegevoegdAlsBeheerderScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningOpvolgersWerdenToegevoegdAlsBeheerder ErkenningOpvolgersWerdenToegevoegdAlsBeheerder { get; }

    public ErkenningOpvolgersWerdenToegevoegdAlsBeheerderScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.InAanvraag.Value,
        };

        ErkenningOpvolgersWerdenToegevoegdAlsBeheerder =
            AutoFixture.Create<ErkenningOpvolgersWerdenToegevoegdAlsBeheerder>() with
            {
                ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerd,
                ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
            ),
        ];
}
