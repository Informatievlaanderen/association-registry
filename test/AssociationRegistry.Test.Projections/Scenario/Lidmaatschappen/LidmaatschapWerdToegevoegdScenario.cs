namespace AssociationRegistry.Test.Projections.Scenario.Lidmaatschappen;

using Events;
using AutoFixture;

public class LidmaatschapWerdToegevoegdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegdFirst { get; set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegdSecond { get; set; }

    public LidmaatschapWerdToegevoegdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        LidmaatschapWerdToegevoegdFirst = AutoFixture.Create<LidmaatschapWerdToegevoegd>() with
        {
            VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
        };

        LidmaatschapWerdToegevoegdSecond = AutoFixture.Create<LidmaatschapWerdToegevoegd>() with
        {
            VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Lidmaatschap = AutoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = LidmaatschapWerdToegevoegdFirst.Lidmaatschap.LidmaatschapId + 1,
            }
        };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, LidmaatschapWerdToegevoegdFirst, LidmaatschapWerdToegevoegdSecond),
    ];
}
