namespace AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;

using Events;
using AutoFixture;

public class SubverenigingWerdGewijzigdScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging { get; }
    public SubverenigingRelatieWerdGewijzigd SubverenigingRelatieWerdGewijzigd { get; }
    public SubverenigingDetailsWerdenGewijzigd SubverenigingDetailsWerdenGewijzigd { get; }

    public SubverenigingWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarSubvereniging =
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
               AutoFixture.Create<Registratiedata.SubverenigingVan>());

        SubverenigingRelatieWerdGewijzigd = AutoFixture.Create<SubverenigingRelatieWerdGewijzigd>();
        SubverenigingDetailsWerdenGewijzigd = AutoFixture.Create<SubverenigingDetailsWerdenGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarSubvereniging,
            SubverenigingRelatieWerdGewijzigd,
            SubverenigingDetailsWerdenGewijzigd),
    ];
}
