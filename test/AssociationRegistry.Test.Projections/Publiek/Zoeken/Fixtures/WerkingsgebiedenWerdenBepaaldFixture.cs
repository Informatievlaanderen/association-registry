﻿namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Framework;
using Framework.Fixtures;
using Public.Schema.Search;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenBepaaldScenario,
    VerenigingZoekDocument,
    ProjectionContext>(context)
{
    public override async Task<VerenigingZoekDocument> GetResultAsync(WerkingsgebiedenWerdenBepaaldScenario scenario)
    {
        var getResponse =
            await Context
                 .PublicElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenBepaald.VCode);

        return getResponse.Source;
    }
}
