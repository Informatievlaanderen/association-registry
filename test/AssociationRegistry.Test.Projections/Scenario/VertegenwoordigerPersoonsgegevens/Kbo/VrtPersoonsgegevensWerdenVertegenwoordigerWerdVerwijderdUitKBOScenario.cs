namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdVerwijderdUitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO { get; set; }
    public VertegenwoordigerWerdVerwijderdUitKBO VertegenwoordigerWerdVerwijderdUitKBO { get; set; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdVerwijderdUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerWerdOvergenomenUitKBO = AutoFixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>();
        VertegenwoordigerIdDieGeanonimiseerdWerd = VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId;
        VertegenwoordigerWerdVerwijderdUitKBO = AutoFixture.Create<VertegenwoordigerWerdVerwijderdUitKBO>() with
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
                VertegenwoordigerWerdOvergenomenUitKBO,
                VertegenwoordigerWerdVerwijderdUitKBO,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
