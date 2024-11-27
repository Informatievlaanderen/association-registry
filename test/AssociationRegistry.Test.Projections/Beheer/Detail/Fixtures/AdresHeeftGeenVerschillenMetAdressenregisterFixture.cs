namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class AdresHeeftGeenVerschillenMetAdressenregisterFixture(ProjectionContext context)
    : ScenarioFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(AdresHeeftGeenVerschillenMetAdressenregisterScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.VerenigingWerdGeregistreerd.VCode);
}
