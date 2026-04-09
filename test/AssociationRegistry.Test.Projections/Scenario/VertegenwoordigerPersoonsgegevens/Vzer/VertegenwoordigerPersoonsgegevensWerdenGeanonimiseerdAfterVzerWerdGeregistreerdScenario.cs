namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVzerWerdGeregistreerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVzerWerdGeregistreerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerIdDieGeanonimiseerdWerd = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
            .Vertegenwoordigers.First()
            .VertegenwoordigerId;

        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd =
            AutoFixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
            {
                VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
