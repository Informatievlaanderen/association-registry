namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using AutoFixture;
using Events;

public class VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdToegevoegd = AutoFixture.Create<VertegenwoordigerWerdToegevoegd>();
        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd =
            AutoFixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
            {
                VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VertegenwoordigerWerdToegevoegd,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
