namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdScenario
    : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; set; }

    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdScenario()
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
