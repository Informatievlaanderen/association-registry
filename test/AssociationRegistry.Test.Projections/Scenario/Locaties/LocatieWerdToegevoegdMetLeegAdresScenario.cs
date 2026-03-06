namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class LocatieWerdToegevoegdMetLeegAdresScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd { get; }

    public LocatieWerdToegevoegdMetLeegAdresScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        LocatieWerdToegevoegd = AutoFixture.Create<LocatieWerdToegevoegd>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with { Adres = null },
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                LocatieWerdToegevoegd
            ),
        ];
}
