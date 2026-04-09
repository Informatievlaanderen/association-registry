namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Fv;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterFvWerdGeregistreerdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterFvWerdGeregistreerdScenario()
    {
        FeitelijkeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VertegenwoordigerIdDieGeanonimiseerdWerd = FeitelijkeVerenigingWerdGeregistreerd
            .Vertegenwoordigers.First()
            .VertegenwoordigerId;

        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd =
            AutoFixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
            {
                VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
            };
    }

    public override string AggregateId => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                FeitelijkeVerenigingWerdGeregistreerd,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
