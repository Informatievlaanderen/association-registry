namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class LidmaatschapWerdToegevoegdScenario : ScenarioBase
{
    public string VCodeDochter { get; private set; }
    public string VCodeMoeder { get; private set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingenwerdenGeregistreerdDochter { get; }
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingenwerdenGeregistreerdMoeder { get; }

    public LidmaatschapWerdToegevoegdScenario()
    {
        VerenigingenwerdenGeregistreerdDochter = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        VerenigingenwerdenGeregistreerdMoeder = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VCodeDochter = VerenigingenwerdenGeregistreerdDochter.VCode;
        VCodeMoeder = VerenigingenwerdenGeregistreerdMoeder.VCode;

        LidmaatschapWerdToegevoegd = AutoFixture.Create<LidmaatschapWerdToegevoegd>() with
        {
            VCode = VCodeDochter,
            Lidmaatschap = AutoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                AndereVereniging = VCodeMoeder,
            },
        };
    }

    public override string VCode => VerenigingenwerdenGeregistreerdDochter.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingenwerdenGeregistreerdMoeder.VCode, VerenigingenwerdenGeregistreerdMoeder),
        new(VCode, VerenigingenwerdenGeregistreerdDochter, LidmaatschapWerdToegevoegd),
    ];
}
