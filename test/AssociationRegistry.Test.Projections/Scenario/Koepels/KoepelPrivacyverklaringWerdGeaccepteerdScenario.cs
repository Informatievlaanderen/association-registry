namespace AssociationRegistry.Test.Projections.Scenario.Koepels;

using AssociationRegistry.Events;
using AutoFixture;

public class KoepelPrivacyverklaringWerdGeaccepteerdScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdGeidentificeerdAlsKoepel VerenigingWerdGeidentificeerdAlsKoepel { get; }
    public KoepelPrivacyverklaringWerdGeaccepteerd KoepelPrivacyverklaringWerdGeaccepteerd { get; }

    public KoepelPrivacyverklaringWerdGeaccepteerdScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        var vCode = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

        VerenigingWerdGeidentificeerdAlsKoepel = new VerenigingWerdGeidentificeerdAlsKoepel(vCode);
        KoepelPrivacyverklaringWerdGeaccepteerd = new KoepelPrivacyverklaringWerdGeaccepteerd(vCode);
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdGeidentificeerdAlsKoepel,
                KoepelPrivacyverklaringWerdGeaccepteerd
            ),
        ];
}
