namespace AssociationRegistry.Test.Projections.Scenario.Subtypes;

using AutoFixture;
using Events;

public class SubverenigingWerdTerugGezetNaarNietBepaaldScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging { get; set; }
    public VerenigingssubtypeWerdTerugGezetNaarNietBepaald VerenigingssubtypeWerdTerugGezetNaarNietBepaald { get; set; }



    public SubverenigingWerdTerugGezetNaarNietBepaaldScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = AutoFixture.Create<VerenigingssubtypeWerdVerfijndNaarSubvereniging>();

        VerenigingssubtypeWerdTerugGezetNaarNietBepaald =
            AutoFixture.Create<VerenigingssubtypeWerdTerugGezetNaarNietBepaald>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingssubtypeWerdVerfijndNaarSubvereniging, VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
    ];
}
