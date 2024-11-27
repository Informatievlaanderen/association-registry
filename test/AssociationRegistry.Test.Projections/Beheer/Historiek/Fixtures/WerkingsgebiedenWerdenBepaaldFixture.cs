﻿namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenBepaaldScenario,
    BeheerVerenigingHistoriekDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(WerkingsgebiedenWerdenBepaaldScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingHistoriekDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenBepaald.VCode);
}
