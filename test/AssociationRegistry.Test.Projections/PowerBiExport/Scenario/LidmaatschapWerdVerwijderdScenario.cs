namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;
using Framework.Fixtures;

public class LidmaatschapWerdVerwijderdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public string VCodeDochter { get; private set; }
    public string VCodeMoeder { get; private set; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; private set; }
    public LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd { get; private set; }
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public LidmaatschapWerdVerwijderdScenario(ProjectionContext context) : base(context)
    {
        VerenigingenwerdenGeregistreerd = AutoFixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>()
                                                     .ToArray();
    }

    public override async Task Given()
    {
        await using var session = await Context.GetDocumentSession();

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

        LidmaatschapWerdVerwijderd = AutoFixture.Create<LidmaatschapWerdVerwijderd>() with
        {
            VCode = VCodeDochter,
            Lidmaatschap = LidmaatschapWerdToegevoegd.Lidmaatschap,
        };

        session.Events.Append(VCodeDochter, LidmaatschapWerdToegevoegd, LidmaatschapWerdVerwijderd);

        await session.SaveChangesAsync();
    }
}
