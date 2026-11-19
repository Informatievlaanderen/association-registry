namespace AssociationRegistry.Test.Projections.Scenario.Koepels;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingWerdGeidentificeerdAlsKoepelScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdGeidentificeerdAlsKoepel VerenigingWerdGeidentificeerdAlsKoepel { get; }

    public VerenigingWerdGeidentificeerdAlsKoepelScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdGeidentificeerdAlsKoepel = new VerenigingWerdGeidentificeerdAlsKoepel(
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode
        );
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdGeidentificeerdAlsKoepel
            ),
        ];
}
