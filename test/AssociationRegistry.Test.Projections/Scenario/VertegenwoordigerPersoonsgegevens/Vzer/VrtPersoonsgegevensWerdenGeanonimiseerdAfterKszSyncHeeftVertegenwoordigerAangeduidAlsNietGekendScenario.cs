namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VrtPersoonsgegevensWerdenGeanonimiseerdAfterKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario
    : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }

    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; set; }

    public KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend { get; set; }

    public VrtPersoonsgegevensWerdenGeanonimiseerdAfterKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerIdDieGeanonimiseerdWerd = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
            .Vertegenwoordigers.First()
            .VertegenwoordigerId;

        KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend =
            AutoFixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend>() with
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
                KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}
