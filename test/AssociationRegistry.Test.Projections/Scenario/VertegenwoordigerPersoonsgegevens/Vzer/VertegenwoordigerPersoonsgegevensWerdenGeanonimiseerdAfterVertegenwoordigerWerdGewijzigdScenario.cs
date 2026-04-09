namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdScenario
    : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }
    public VertegenwoordigerWerdGewijzigd VertegenwoordigerWerdGewijzigd { get; }
    public VertegenwoordigerWerdGewijzigd VertegenwoordigerOpnieuwGewijzigd { get; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerIdDieGeanonimiseerdWerd = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
            .Vertegenwoordigers.First()
            .VertegenwoordigerId;

        VertegenwoordigerWerdGewijzigd = AutoFixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
        };

        VertegenwoordigerOpnieuwGewijzigd = AutoFixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
        };

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
                VertegenwoordigerWerdGewijzigd,
                VertegenwoordigerOpnieuwGewijzigd,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
