namespace AssociationRegistry.Test.Projections.Scenario.Koepels;

using AssociationRegistry.Events;
using AutoFixture;

public class KoepelstatusWerdTerugGezetNaarNietBepaaldScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdGeidentificeerdAlsKoepel VerenigingWerdGeidentificeerdAlsKoepel { get; }
    public KoepelstatusWerdTerugGezetNaarNietBepaald KoepelstatusWerdTerugGezetNaarNietBepaald { get; }

    public KoepelstatusWerdTerugGezetNaarNietBepaaldScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        var vCode = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

        VerenigingWerdGeidentificeerdAlsKoepel = new VerenigingWerdGeidentificeerdAlsKoepel(vCode);
        KoepelstatusWerdTerugGezetNaarNietBepaald = new KoepelstatusWerdTerugGezetNaarNietBepaald(vCode);
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdGeidentificeerdAlsKoepel,
                KoepelstatusWerdTerugGezetNaarNietBepaald
            ),
        ];
}
