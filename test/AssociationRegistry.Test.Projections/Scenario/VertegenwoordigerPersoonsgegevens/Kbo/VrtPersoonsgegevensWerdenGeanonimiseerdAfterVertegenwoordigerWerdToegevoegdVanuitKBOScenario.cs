namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdVanuitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; set; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdVanuitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerWerdToegevoegdVanuitKBO = AutoFixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        VertegenwoordigerIdDieGeanonimiseerdWerd = VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId;

        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd =
            AutoFixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
            {
                VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
            };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VertegenwoordigerWerdToegevoegdVanuitKBO,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
