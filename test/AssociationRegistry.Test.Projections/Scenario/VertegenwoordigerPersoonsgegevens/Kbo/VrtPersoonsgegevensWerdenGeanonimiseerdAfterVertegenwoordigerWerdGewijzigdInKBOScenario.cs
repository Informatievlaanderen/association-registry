namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdInKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; set; }
    public VertegenwoordigerWerdGewijzigdInKBO VertegenwoordigerWerdGewijzigdInKBO { get; set; }
    public VertegenwoordigerWerdGewijzigdInKBO VertegenwoordigerWerdOpnieuwGewijzigdInKBO { get; set; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdInKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerWerdToegevoegdVanuitKBO = AutoFixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        VertegenwoordigerIdDieGeanonimiseerdWerd = VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId;
        VertegenwoordigerWerdGewijzigdInKBO = AutoFixture.Create<VertegenwoordigerWerdGewijzigdInKBO>() with
        {
            VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
        };
        VertegenwoordigerWerdOpnieuwGewijzigdInKBO = AutoFixture.Create<VertegenwoordigerWerdGewijzigdInKBO>() with
        {
            VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
        };

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
                VertegenwoordigerWerdGewijzigdInKBO,
                VertegenwoordigerWerdOpnieuwGewijzigdInKBO,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
