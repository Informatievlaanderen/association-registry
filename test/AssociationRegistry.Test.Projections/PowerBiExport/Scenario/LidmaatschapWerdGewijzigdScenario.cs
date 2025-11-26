namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class LidmaatschapWerdGewijzigdScenario : ScenarioBase
{
    public string VCodeDochter { get; }
    public string VCodeMoeder { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingenwerdenGeregistreerdDochter { get; }
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingenwerdenGeregistreerdMoeder { get; }

    public LidmaatschapWerdGewijzigdScenario()
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

        LidmaatschapWerdGewijzigd = AutoFixture.Create<LidmaatschapWerdGewijzigd>() with
        {
            VCode = VCodeDochter,
            Lidmaatschap = AutoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                AndereVereniging = VCodeMoeder,
            },
        };
    }

    public override string AggregateId => VerenigingenwerdenGeregistreerdDochter.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingenwerdenGeregistreerdMoeder.VCode, VerenigingenwerdenGeregistreerdMoeder),
        new(AggregateId, VerenigingenwerdenGeregistreerdDochter, LidmaatschapWerdToegevoegd, LidmaatschapWerdGewijzigd),
    ];
}
