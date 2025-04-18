namespace AssociationRegistry.Test.Projections.Scenario.Subtypes;

using AutoFixture;
using Events;

public class SubverenigingWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging { get; set; }
    public SubverenigingRelatieWerdGewijzigd SubverenigingRelatieWerdGewijzigd { get; set; }
    public SubverenigingDetailsWerdenGewijzigd SubverenigingDetailsWerdenGewijzigd { get; set; }



    public SubverenigingWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = AutoFixture.Create<VerenigingssubtypeWerdVerfijndNaarSubvereniging>();

        SubverenigingRelatieWerdGewijzigd =
            AutoFixture.Create<SubverenigingRelatieWerdGewijzigd>();

        SubverenigingDetailsWerdenGewijzigd =
            AutoFixture.Create<SubverenigingDetailsWerdenGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingssubtypeWerdVerfijndNaarSubvereniging, SubverenigingRelatieWerdGewijzigd, SubverenigingDetailsWerdenGewijzigd),
    ];
}
