namespace AssociationRegistry.Test.Projections.Scenario.Subtypes;

using AutoFixture;
using Events;

public class SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging { get; set; }
    public VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; set; }



    public SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = AutoFixture.Create<VerenigingssubtypeWerdVerfijndNaarSubvereniging>();

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging =
            AutoFixture.Create<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingssubtypeWerdVerfijndNaarSubvereniging, VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
    ];
}
