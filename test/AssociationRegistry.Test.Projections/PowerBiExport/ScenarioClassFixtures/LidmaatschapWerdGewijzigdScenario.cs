namespace AssociationRegistry.Test.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Events;
using Framework.Fixtures;

public class LidmaatschapWerdGewijzigdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public string VCodeDochter { get; private set; }
    public string VCodeMoeder { get; private set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; private set; }
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; private set; }
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public LidmaatschapWerdGewijzigdScenario(PowerBiExportContext context) : base(context)
    {
        VerenigingenwerdenGeregistreerd = AutoFixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>()
                                                     .ToArray();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        foreach (var feitelijkeVerenigingWerdGeregistreerd in VerenigingenwerdenGeregistreerd)
        {
            session.Events.Append(feitelijkeVerenigingWerdGeregistreerd.VCode, feitelijkeVerenigingWerdGeregistreerd);
        }

        VCodeDochter = VerenigingenwerdenGeregistreerd[0].VCode;
        VCodeMoeder = VerenigingenwerdenGeregistreerd[1].VCode;

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

        session.Events.Append(VCodeDochter, LidmaatschapWerdToegevoegd, LidmaatschapWerdGewijzigd);

        await session.SaveChangesAsync();
    }
}
