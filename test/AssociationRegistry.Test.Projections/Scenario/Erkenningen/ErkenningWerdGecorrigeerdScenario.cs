namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using Events;

public class ErkenningWerdGecorrigeerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGewijzigd ErkenningWerdGewijzigd { get; }

    public ErkenningWerdGecorrigeerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdGewijzigd =
            AutoFixture.Create<ErkenningWerdGewijzigd>() with
            {
                ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
                Startdatum = ErkenningWerdGeregistreerd.Startdatum.Value.AddDays(10),
                Einddatum = ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(10),
                Hernieuwingsdatum= ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value.AddDays(10),
                HernieuwingsUrl="https://a-new-url-is-generated.random" + AutoFixture.Create<Guid>(),
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerd,
                ErkenningWerdGewijzigd
            ),
        ];
}
