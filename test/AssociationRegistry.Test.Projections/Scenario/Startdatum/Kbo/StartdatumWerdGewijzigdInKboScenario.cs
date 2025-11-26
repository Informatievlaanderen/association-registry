namespace AssociationRegistry.Test.Projections.Scenario.Startdatum.Kbo;

using Events;
using AutoFixture;

public class StartdatumWerdGewijzigdInKboScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public StartdatumWerdGewijzigdInKbo StartdatumWerdGewijzigdInKbo { get; }

    public StartdatumWerdGewijzigdInKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        StartdatumWerdGewijzigdInKbo = AutoFixture.Create<StartdatumWerdGewijzigdInKbo>();
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            StartdatumWerdGewijzigdInKbo
        ),
    ];
}
