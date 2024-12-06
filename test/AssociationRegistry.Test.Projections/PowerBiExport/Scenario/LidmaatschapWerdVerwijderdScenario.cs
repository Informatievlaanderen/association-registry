namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;
using Framework.Fixtures;

public class LidmaatschapWerdVerwijderdScenario : ScenarioBase
{
    public string VCodeDochter { get; private set; }
    public string VCodeMoeder { get; private set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }
    public LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingenwerdenGeregistreerdDochter { get; }
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingenwerdenGeregistreerdMoeder { get; }

    public LidmaatschapWerdVerwijderdScenario()
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

        LidmaatschapWerdVerwijderd = AutoFixture.Create<LidmaatschapWerdVerwijderd>() with
        {
            VCode = VCodeDochter,
            Lidmaatschap = AutoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                AndereVereniging = VCodeMoeder,
            },
        };
    }

    public override string VCode => VerenigingenwerdenGeregistreerdDochter.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingenwerdenGeregistreerdMoeder.VCode, VerenigingenwerdenGeregistreerdMoeder),
        new(VCode, VerenigingenwerdenGeregistreerdDochter, LidmaatschapWerdToegevoegd, LidmaatschapWerdVerwijderd),
    ];
}
