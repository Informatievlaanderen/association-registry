namespace AssociationRegistry.Test.Projections.Scenario.Lidmaatschappen;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class LidmaatschapWerdToegevoegdMetNullDatesScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegdWithNullDates { get; set; }

    public LidmaatschapWerdToegevoegdMetNullDatesScenario()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = autoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        LidmaatschapWerdToegevoegdWithNullDates = autoFixture.Create<LidmaatschapWerdToegevoegd>() with
        {
            VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Lidmaatschap = autoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                DatumVan = null,
                DatumTot = null,
            }
        };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, LidmaatschapWerdToegevoegdWithNullDates),
    ];
}
