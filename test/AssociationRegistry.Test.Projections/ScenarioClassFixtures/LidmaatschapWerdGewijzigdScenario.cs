﻿namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework;
using Framework.Fixtures;

public class LidmaatschapWerdGewijzigdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; }

    public LidmaatschapWerdGewijzigdScenario(ProjectionContext context) : base(context)
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new LidmaatschapWerdToegevoegdScenario(context);
        scenario.Given().GetAwaiter().GetResult();

        LidmaatschapWerdGewijzigd = new LidmaatschapWerdGewijzigd(
            Lidmaatschap: fixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            },
            VCode: scenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(LidmaatschapWerdGewijzigd.VCode,
                              LidmaatschapWerdGewijzigd);

        await session.SaveChangesAsync();
    }
}
