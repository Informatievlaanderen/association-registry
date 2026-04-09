namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VrtPersoonsgegevensWerdenGeanonimiseerdAfterKszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario
    : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden { get; set; }

    public VrtPersoonsgegevensWerdenGeanonimiseerdAfterKszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerIdDieGeanonimiseerdWerd = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
            .Vertegenwoordigers.First()
            .VertegenwoordigerId;

        KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden =
            AutoFixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden>() with
            {
                VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
            };

        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd =
            AutoFixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
            {
                VertegenwoordigerId = VertegenwoordigerIdDieGeanonimiseerdWerd,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
