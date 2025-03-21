﻿namespace AssociationRegistry.Test.Projections.Publiek.Detail.Werkingsgebieden;

using JsonLdContext;
using Public.Schema.Detail;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd(PubliekDetailScenarioFixture<WerkingsgebiedenWerdenGewijzigdScenario> fixture)
    : PubliekDetailScenarioClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEquivalentTo(
                       fixture.Scenario
                              .WerkingsgebiedenWerdenGewijzigd
                              .Werkingsgebieden
                              .Select(wg => new PubliekVerenigingDetailDocument.Types.Werkingsgebied
                               {
                                   JsonLdMetadata = new JsonLdMetadata(
                                       JsonLdType.Werkingsgebied.CreateWithIdValues(wg.Code),
                                       JsonLdType.Werkingsgebied.Type),
                                   Code = wg.Code,
                                   Naam = wg.Naam,
                               }));
}
