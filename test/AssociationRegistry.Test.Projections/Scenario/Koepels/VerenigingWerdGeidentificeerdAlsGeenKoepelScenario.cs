namespace AssociationRegistry.Test.Projections.Scenario.Koepels;

using AssociationRegistry.Events;
using AutoFixture;

/// <summary>
/// Een koepel met geaccepteerde privacyverklaring wordt geïdentificeerd als niet-koepel, waardoor de privacyverklaring wordt ingetrokken.
/// </summary>
public class VerenigingWerdGeidentificeerdAlsGeenKoepelScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdGeidentificeerdAlsKoepel VerenigingWerdGeidentificeerdAlsKoepel { get; }
    public KoepelPrivacyverklaringWerdGeaccepteerd KoepelPrivacyverklaringWerdGeaccepteerd { get; }
    public VerenigingWerdGeidentificeerdAlsGeenKoepel VerenigingWerdGeidentificeerdAlsGeenKoepel { get; }
    public KoepelPrivacyverklaringWerdIngetrokken KoepelPrivacyverklaringWerdIngetrokken { get; }

    public VerenigingWerdGeidentificeerdAlsGeenKoepelScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        var vCode = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

        VerenigingWerdGeidentificeerdAlsKoepel = new VerenigingWerdGeidentificeerdAlsKoepel(vCode);
        KoepelPrivacyverklaringWerdGeaccepteerd = new KoepelPrivacyverklaringWerdGeaccepteerd(vCode);
        VerenigingWerdGeidentificeerdAlsGeenKoepel = new VerenigingWerdGeidentificeerdAlsGeenKoepel(vCode);
        KoepelPrivacyverklaringWerdIngetrokken = new KoepelPrivacyverklaringWerdIngetrokken(vCode);
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdGeidentificeerdAlsKoepel,
                KoepelPrivacyverklaringWerdGeaccepteerd,
                VerenigingWerdGeidentificeerdAlsGeenKoepel,
                KoepelPrivacyverklaringWerdIngetrokken
            ),
        ];
}
