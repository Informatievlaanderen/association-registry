namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class LocatieWerdToegevoegdMaatschappelijkeZetelScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd { get; }

    public LocatieWerdToegevoegdMaatschappelijkeZetelScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        LocatieWerdToegevoegd = AutoFixture.Create<LocatieWerdToegevoegd>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
            {
                Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
            },
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
